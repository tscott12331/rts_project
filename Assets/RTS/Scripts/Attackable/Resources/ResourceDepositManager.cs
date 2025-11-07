using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class ResourceDepositManager : MonoBehaviourSingleton<ResourceDepositManager>
{
    public Transform PlayerResources;
    public Transform EnemyResources;

    public Dictionary<string, ResourceDepositSO> ResourceDepositDict { get; private set; } = new();
    public void LoadResourceDeposits()
    {
        var rdSOs = Resources.LoadAll<ResourceDepositSO>("ScriptableObjects/ResourceDeposits/");

        bool haveYtalnium = false, haveNM = false;
        for (int i = 0; i < rdSOs.Length; i++)
        {
            var so = rdSOs[i];
            var type = so.Data.RType;
            if(type == ResourceType.Ytalnium && !haveYtalnium)
            {
                ResourceDepositDict.Add("Ytalnium", so);
                Dbx.CtxLog($"Loaded Ytalnium resource as {so.Data.Prefab.name}");
                haveYtalnium = true;
            } else if(type == ResourceType.NaturalMetal && !haveNM)
            {
                ResourceDepositDict.Add("NaturalMetal", so);
                Dbx.CtxLog($"Loaded Natural Metal resource as {so.Data.Prefab.name}");
                haveNM = true;
            }
        }

        if (!haveYtalnium || !haveNM) throw new System.Exception("Could not find data for all resource types");

        PlaceResources();
    }

    public void PlaceResources()
    {
        if (!ResourceDepositDict.ContainsKey("Ytalnium")
            || !ResourceDepositDict.ContainsKey("NaturalMetal"))
        {
            Dbx.CtxLog("Not all resource types are loaded");
            return;
        }

        CreateResourcesFromOwnerResourceContainer(PlayerResources);
        CreateResourcesFromOwnerResourceContainer(EnemyResources);
    }

    public void CreateResourcesFromContainerOfType(Transform container, ResourceType RType)
    {
        if (container == null)
        {
            Dbx.CtxLog("Container is null");
            return;
        }

        for (int i = 0; i < container.childCount; i++)
        {
            var depositTransform = container.GetChild(i);
            CreateResource(depositTransform, RType);
        }
    }

    public void CreateResourcesFromOwnerResourceContainer(Transform container)
    {
        if (container == null)
        {
            Dbx.CtxLog("Container is null");
            return;
        }

        var ytalniumContainer = container.Find("Ytalnium");
        CreateResourcesFromContainerOfType(ytalniumContainer, ResourceType.Ytalnium);

        var nmContainer = container.Find("Natural Metal");
        CreateResourcesFromContainerOfType(nmContainer, ResourceType.NaturalMetal);
    }

    public void CreateResource(Transform position, ResourceType RType)
    {
        if (!ResourceDepositDict.ContainsKey("Ytalnium")
            || !ResourceDepositDict.ContainsKey("NaturalMetal"))
        {
            Dbx.CtxLog("Not all resource types are loaded");
            return;
        }

        var so = RType == ResourceType.Ytalnium 
                ? ResourceDepositDict["Ytalnium"] 
                : ResourceDepositDict["NaturalMetal"];

        var go = Instantiate(so.Data.Prefab, position);
        go.TryGetComponent<ResourceDeposit>(out var deposit);
        if(deposit == null)
        {
            Dbx.CtxLog("Resource prefab does not have ResourceDeposit script");
            return;
        }

        deposit.CopyData(so);
    }

    public void CollectorUnit_ResourceDepositDestroyed(ResourceDeposit deposit)
    {
        Destroy(deposit.gameObject);
    }

    private void OnEnable()
    {
        CollectorUnit.ResourceDepositDestroyed += CollectorUnit_ResourceDepositDestroyed;
    }

    private void OnDisable()
    {
        CollectorUnit.ResourceDepositDestroyed -= CollectorUnit_ResourceDepositDestroyed;
    }
}
