﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;


public class RandomRoadSinglePlayer : MonoBehaviour
{
    public static string direction;
    public static int n = 0;
    static Vector3 vector3 = new Vector3(0f, 0f, 0f);
    public static Quaternion rotator;
    private static Transform next_road_position;
    private WaypointRoute myroute;
    private List<GameObject> roadss;
    private float currentspawn;
    private float time;
    private float percentage = 0.8f;
    static bool flag = true;
    float elapsed = 0f;  

    [Header("Time for spawning road")] 
    public float minSpeed;
    public float overTime;    
    public float maxSpeed ;
    [Header("Road Deletion")]
    public int roadcheck;
    public int nroaddelete;
    private int generatedRoads = 0;


    /* Array filled with:
     * 0 -> easy,
     * 1 -> medium,
     * 2 -> hard
     */
    private List<int> lotteryTickets;

    [SerializeField] private int startDifficultTurn;
    [SerializeField] private int speedDifficultyMultiplier;


    [Header("Roads")]
    [SerializeField] private GameObject[] myArray;


    // Start is     called before the first frame update
    private void Awake()
    {
        setDifficulty();
        next_road_position = GetComponent<Transform>();
        elapsed = Time.timeSinceLevelLoad;
        time = Time.time;
        next_road_position.position = vector3;
        rotator = Quaternion.Euler(-90f, 0f, 90f);
        //        _tr.GetComponents<Transform>();
        roadss = new List<GameObject>();
        currentspawn = maxSpeed;
        lotteryTickets = new List<int>();

        for(int i=0; i<100; i++)
        {
            lotteryTickets.Add(0);
        }


    }

    

    // Update is called once per frame
    void Update()
    {
        currentspawn = Mathf.SmoothStep(maxSpeed, minSpeed, (Time.time - time) / overTime );
        elapsed += Time.deltaTime;
            if (elapsed >= currentspawn)
            {
                elapsed = 0f;
                GenRoad();
            }
        
        

    }
     void GenRoad()
    {
        if (gameObject.transform.GetChild(0).gameObject.activeSelf == false)
        {
            bool flag = true;

            for (int i = 0; i < speedDifficultyMultiplier; i++)
                lotteryTickets.Add(1);

            if(generatedRoads>=startDifficultTurn)
            {
                for (int i = 0; i < speedDifficultyMultiplier; i++)
                    lotteryTickets.Add(2);
            }

            int index_difficulty = UnityEngine.Random.Range(0, lotteryTickets.Count);
            int difficulty = lotteryTickets[index_difficulty];

            int startIndex = difficulty * 5;
            int endIndex = difficulty * 5 + 5;

            //int value = checkcurve();
            int value = UnityEngine.Random.Range(startIndex, endIndex);

            while (flag)
            {
                value = UnityEngine.Random.Range(startIndex, endIndex);
                GameObject pre = myArray[value];
                if (checkcollision(pre))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }

            }
            
            name_road(value);
            SoundManager.instance.playSingle();
            GameObject go =ObjectPoolingManager.Instance.GetObject(myArray[value].name);
            roadss.Add(go);
            if (Random.value > percentage)
            {
                go.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (myArray[value].name == "loop") {
                go.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            n++;
            go.tag = n.ToString();
            go.transform.position = next_road_position.position;
            go.transform.rotation = rotator;
            next_road_position.position = go.gameObject.transform.GetChild(3).position;
            //checkcollision(go, go.transform.rotation);
            //DeleteRoad.CleanRoadSingle();
            RoadRotator.rotatorsingle();
           // Deletewaypoints();
            CleanRoad(roadcheck,nroaddelete);
            //setwaypoints(go.gameObject.transform.GetChild(0), go.gameObject.transform.GetChild(1),go.gameObject.transform.GetChild(3));
            //myroute.UpdateNodes();

        }
        generatedRoads++;
    }
     

     private void FixedUpdate()
     {
        // checkcollision(next_road_position.position + vector3);
     }

     private bool checkcollision(GameObject pre)
     {
         RaycastHit hit;
         pre.transform.position = next_road_position.position + vector3; 
         pre.transform.rotation = rotator;
         var heading  = pre.transform.GetChild(3).position - pre.transform.GetChild(2).position;
         var distance = heading.magnitude;
         var direction = heading / distance;
         direction *= 50;
         // Vector3 forward = transform.TransformDirection(Vector3.forward) * 50;
         //forward = Quaternion.Euler(direction) * forward;
         Debug.DrawRay(pre.transform.GetChild(3).position, direction, Color.green, 10);
         if (Physics.SphereCast(pre.transform.GetChild(2).position,40f, direction, out hit,80f))
         {
             print("Found an object - distance: " + hit.distance);
             return true;
         }

         return false;

     }
     public void CleanRoad(int maxroad, int delete)
     {
         if (roadss.Count > maxroad)
         {
             for(int i = 0; i < delete;i++)
             {
                 roadss[i].SetActive(false);
             }
             roadss.RemoveRange(0,delete);
         }
     }

     private void setDifficulty()
     {

         switch (Diffulty.Difficulty)
         {
             case "Easy":
                 minSpeed = 1.5f; 
                 overTime = 90f; 
                 maxSpeed = 2.5f; 
                 roadcheck = 15; 
                 nroaddelete = 3;
                 break;
             case "Normal":
                 minSpeed = 1.5f; 
                 overTime = 60f; 
                 maxSpeed = 2.5f; 
                 roadcheck = 15; 
                 nroaddelete = 5;                 
                 break;
             case "Hard":
                 minSpeed = 1.0f; 
                 overTime = 90f; 
                 maxSpeed = 2.0f; 
                 roadcheck = 10; 
                 nroaddelete = 4;                 
                 break;
             case "Insane":
                 minSpeed = 0.8f; 
                 overTime = 90f; 
                 maxSpeed = 1.5f; 
                 roadcheck = 8; 
                 nroaddelete = 3;                 
                 break;
             default:
                 minSpeed = 1.0f; 
                 overTime = 90f; 
                 maxSpeed = 1.5f; 
                 roadcheck = 15; 
                 nroaddelete = 8;                      
                 break;
         }
         
     }
//     private void Deletewaypoints()
//     {
//         if (waypoints.transform.childCount > 50)
//         {
//             for (int i = 0; i < 3; i++)
//             {
//                 GameObject.Destroy(waypoints.transform.GetChild(i).gameObject);
//
//             }
//
//         }
//
//     }
//
//     private void setwaypoints(Transform waypoint1, Transform waypoint2,Transform waypoint3)
//     {
//         waypoint1.SetParent(waypoints.transform);
//         waypoint2.SetParent(waypoints.transform);
//         waypoint3.SetParent(waypoints.transform);
//         
//     }
     private void name_road(int i)
     {
         direction= myArray[i].name.ToString();
        print(direction);
     }
}
