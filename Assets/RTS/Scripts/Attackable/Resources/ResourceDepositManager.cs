using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class ResourceDepositManager : MonoBehaviourSingleton<ResourceDepositManager>
{
    public Transform PlayerResources;
    public Transform EnemyResources;

    // holds resoure deposit scriptable objects
    public Dictionary<string, ResourceDepositSO> ResourceDepositDict { get; private set; } = new();
    public void LoadResourceDeposits()
    {
        // load from resources
        var rdSOs = Resources.LoadAll<ResourceDepositSO>("ScriptableObjects/ResourceDeposits/");

        // variable to determine if we've loaded a resource type already
        bool haveYtalnium = false, haveNM = false;
        for (int i = 0; i < rdSOs.Length; i++)
        {
            var so = rdSOs[i];
            var type = so.Data.RType;
            if(type == ResourceType.Ytalnium && !haveYtalnium)
            {
                // add ytalnium resource data to dictionary
                ResourceDepositDict.Add("Ytalnium", so);
                Dbx.CtxLog($"Loaded Ytalnium resource as {so.Data.Prefab.name}");
                haveYtalnium = true;
            } else if(type == ResourceType.NaturalMetal && !haveNM)
            {
                // add natural metal resource data to dictionary
                ResourceDepositDict.Add("NaturalMetal", so);
                Dbx.CtxLog($"Loaded Natural Metal resource as {so.Data.Prefab.name}");
                haveNM = true;
            }
        }

        // throw error if ytalnium and/or natural metal data not provided
        if (!haveYtalnium || !haveNM) throw new System.Exception("Could not find data for all resource types");

        // place resources after loading
        PlaceResources();
    }

    public void PlaceResources()
    {
        // check to see that both resources are loaded
        if (!ResourceDepositDict.ContainsKey("Ytalnium")
            || !ResourceDepositDict.ContainsKey("NaturalMetal"))
        {
            Dbx.CtxLog("Not all resource types are loaded");
            return;
        }

        // create the resources based on game object hierarchy provided through editor
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

        // loop through child transforms and place at that position
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

        // each owner resource container contains an ytalnium and natural metal container

        var ytalniumContainer = container.Find("Ytalnium");
        CreateResourcesFromContainerOfType(ytalniumContainer, ResourceType.Ytalnium);

        var nmContainer = container.Find("Natural Metal");
        CreateResourcesFromContainerOfType(nmContainer, ResourceType.NaturalMetal);
    }

    public void CreateResource(Transform position, ResourceType RType)
    {
        // stop if not all resource types are loaded
        if (!ResourceDepositDict.ContainsKey("Ytalnium")
            || !ResourceDepositDict.ContainsKey("NaturalMetal"))
        {
            Dbx.CtxLog("Not all resource types are loaded");
            return;
        }

        // get scriptable object based on type
        var so = RType == ResourceType.Ytalnium 
                ? ResourceDepositDict["Ytalnium"] 
                : ResourceDepositDict["NaturalMetal"];

        // instantiate deposit
        var go = Instantiate(so.Data.Prefab, position);
        // get ResourceDeposit object associated with the game object
        go.TryGetComponent<ResourceDeposit>(out var deposit);
        if(deposit == null)
        {
            // if the ResourceDeposit object doesn't exist, the prefab is invalid
            Dbx.CtxLog("Resource prefab does not have ResourceDeposit script");
            return;
        }

        // copy the scriptable object data to the newly created deposit
        deposit.CopyData(so);
    }

    public void CollectorUnit_ResourceDepositDestroyed(ResourceDeposit deposit)
    {
        // destroy deposit when a collector has fully depleted it
        Destroy(deposit.gameObject);
    }

    // add and remove listeners
    private void OnEnable()
    {
        CollectorUnit.ResourceDepositDestroyed += CollectorUnit_ResourceDepositDestroyed;
    }

    private void OnDisable()
    {
        CollectorUnit.ResourceDepositDestroyed -= CollectorUnit_ResourceDepositDestroyed;
    }
}
