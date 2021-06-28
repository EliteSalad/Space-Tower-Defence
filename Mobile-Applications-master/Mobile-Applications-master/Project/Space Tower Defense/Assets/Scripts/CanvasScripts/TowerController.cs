using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;

enum TurretTypes { NOTHING, STANDARDTURRET, LASERTURRET, ELECTRICITYTURRET, ROCKETTURRET, SNIPERTURRET}
public class TowerController : MonoBehaviour
{
    
    private int[] Prices = new int[4] { 10, 50, 100, 200 }; //Needs to be removed
    public int Current_Gold = 10; //Needs to be removed

    byte Upgrade_Level = 0;
    byte Max_Upgrade_Level = 4;
    public List<GameObject> Buttons_;

    private Transform Spawned_Tower;
    public GameObject TowerParent;

    public GameObject goldController;
    public List<GameObject> StandardTurrets;
    public List<GameObject> RocketTurrets;
    public List<GameObject> SniperTurrets;
    public List<GameObject> ElectricityTurrets;
    public List<GameObject> LaserTurrets;

    private TurretTypes thisTurretType = TurretTypes.NOTHING;
    public bool buttonsActive = false;
    private void Start()
    {
        goldController = GameObject.Find("GoldText");
        updateGold();
    }

    public void Buttons()
    {
        foreach (GameObject btn in Buttons_)
        {
            if (Upgrade_Level <= Max_Upgrade_Level)
            {
                if (Upgrade_Level == 0 && btn.tag != "Upgrade")
                {
                    if (btn.tag != "Delete")
                    {
                        btn.SetActive(!btn.activeInHierarchy);
                    }
                }
                else if (Upgrade_Level > 0 && Upgrade_Level < Max_Upgrade_Level && btn.tag == "Upgrade")
                {
                        btn.SetActive(!btn.activeInHierarchy);                    
                }
                else if (Upgrade_Level > 0 && Upgrade_Level < Max_Upgrade_Level && btn.tag == "Delete")
                {
                        btn.SetActive(!btn.activeInHierarchy);
                }
                else if (Upgrade_Level == Max_Upgrade_Level && btn.name == "Delete")
                {
                    btn.SetActive(!btn.activeInHierarchy);
                }
            }
        }
       // foreach (GameObject STB in GameObject.FindGameObjectsWithTag("SpawnTowerButton"))
            gameObject.GetComponent<TowerController>().TowerParent.transform.GetChild(1).GetComponent<RangeIndicatorController>().switchRangeIndicatorOn();
    }
    public void HideButtons()
    {
        GameObject.Find("Canvas").GetComponent<MenuScript>().HideButtons();
        gameObject.GetComponent<TowerController>().TowerParent.transform.GetChild(1).GetComponent<RangeIndicatorController>().switchRangeIndicatorOff();
    }
    public void Action(GameObject obj)
    {
        updateGold();
        if ((obj.tag != "Upgrade" || obj.tag != "Delete") && Current_Gold >= Prices[Upgrade_Level])
        {
            InstantiateCorrectTurret(obj);
            Spawned_Tower = Instantiate(obj, Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
            Spawned_Tower.localPosition = Vector3.zero;
            //Debug.Log("Purchased: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
            Current_Gold -= Prices[Upgrade_Level];
            goldController.GetComponent<GoldController>().ChangeGoldAmount(-Prices[Upgrade_Level]);
            ++Upgrade_Level;
            this.transform.parent.GetChild(6).GetChild(0).GetComponent<Text>().text = "UPGRADE: " + Prices[Upgrade_Level];
            //Debug.Log("New Gold amount: " + Current_Gold);
            HideButtons();
            // Perform graphical updates on tower to show creation via Spawned_Tower.transform..etc and take off gold
        }
        else if (obj.tag == "Upgrade" && Current_Gold >= Prices[Upgrade_Level])
        {
            if (Upgrade_Level < Max_Upgrade_Level)
            {
                Destroy(Spawned_Tower.gameObject);
                UpgradeToCorrectTurret();
                Spawned_Tower.localPosition = Vector3.zero;
                // Debug.Log("Upgraded: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
                Current_Gold -= Prices[Upgrade_Level];
                goldController.GetComponent<GoldController>().ChangeGoldAmount(-Prices[Upgrade_Level]);
                //Debug.Log("New Gold amount: " + Current_Gold);
                ++Upgrade_Level;
                this.transform.parent.GetChild(6).GetChild(0).GetComponent<Text>().text = "UPGRADE: " + Prices[Upgrade_Level];
                HideButtons();
                gameObject.GetComponent<TowerController>().TowerParent.transform.GetChild(1).GetComponent<RangeIndicatorController>().switchRangeIndicatorOff();
                // Perform graphical updates on tower to show upgrade via Spawned_Tower.transform..etc and take off gold
            }
        }
        else if (obj.tag == "Delete")
        {
            Destroy(Spawned_Tower.gameObject);
            thisTurretType = TurretTypes.NOTHING;
            obj.SetActive(true);
            //When Deleteing add half the amount of its current upgrade price back to players current gold
            Current_Gold += (Prices[Upgrade_Level-1]/2);
            goldController.GetComponent<GoldController>().ChangeGoldAmount((Prices[Upgrade_Level-1]/2));
            Upgrade_Level = 0;
            this.transform.parent.GetChild(6).GetChild(0).GetComponent<Text>().text = "UPGRADE: " + Prices[Upgrade_Level];
            HideButtons();
        }
    }
    void updateGold()
    {
        Current_Gold = goldController.GetComponent<GoldController>().GetGoldAmount();
    }

    private void InstantiateCorrectTurret(GameObject GO)
    {

        switch (GO.tag)
        {
            case "StandardTurret":
                thisTurretType = TurretTypes.STANDARDTURRET;
                break;

            case "LaserTurret":
                thisTurretType = TurretTypes.LASERTURRET;
                break;

            case "ElectricityTurret":
                thisTurretType = TurretTypes.ELECTRICITYTURRET;
                break;

            case "SniperTurret":
                thisTurretType = TurretTypes.SNIPERTURRET;
                break;

            case "RocketTurret":
                thisTurretType = TurretTypes.ROCKETTURRET;
                break;
        }
    }

    private void UpgradeToCorrectTurret()
    {
        switch(thisTurretType)
        {
            case TurretTypes.STANDARDTURRET:
                Spawned_Tower = Instantiate(StandardTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
               // Debug.Log(StandardTurrets[Upgrade_Level].name);
                break;

            case TurretTypes.ROCKETTURRET:
                Spawned_Tower = Instantiate(RocketTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
               // Debug.Log(RocketTurrets[Upgrade_Level].name);
                break;

            case TurretTypes.LASERTURRET:
                Spawned_Tower = Instantiate(LaserTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
               // Debug.Log(LaserTurrets[Upgrade_Level].name);
                break;

            case TurretTypes.ELECTRICITYTURRET:
                Spawned_Tower = Instantiate(ElectricityTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
               // Debug.Log(ElectricityTurrets[Upgrade_Level].name);
                break;

            case TurretTypes.SNIPERTURRET:
                Spawned_Tower = Instantiate(SniperTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
               // Debug.Log(SniperTurrets[Upgrade_Level].name);
                break;
        }
    }
}
