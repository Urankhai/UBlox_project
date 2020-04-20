using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class LoS_visualization : MonoBehaviour
{
    private LineRenderer lr1, lr2;
    public Transform Antenna2;
    public Transform ant_tx;

    // Frequency in 802.11p 5.9GHz
    private readonly double Fc = 5.9*1000000000; // Hz
    private readonly double c = 300000000;


    void Update()
    {
        double Lambda = c / Fc;

        // coordinates of antennas
        Vector3 pos_Rx = Antenna2.position;
        Vector3 pos_Tx = ant_tx.position;

        // direction from Tx to Rx
        Vector3 dir_Tx2Rx = pos_Rx - pos_Tx;

        float distance = (pos_Rx - pos_Tx).magnitude;

        // Free space attenuation
        double attenuation = (Lambda / (4*Mathf.PI*distance)) * (Lambda / (4 * Mathf.PI * distance));

        Debug.Log("Distance between antennas = " + distance + "; FSA = " + attenuation);

        // TODO: write a script/code that writes this info to a CSV file.
        string cvpath = "C:\\Users\\hmoh\\Desktop\\V2X Documents\\test.csv";
        File.AppendAllText(cvpath,
                           distance.ToString() + "; " + attenuation.ToString() + Environment.NewLine);
     
        


        if (Physics.Linecast(pos_Tx, pos_Rx))
        {
            Debug.DrawLine(pos_Tx, pos_Rx, Color.red);
        }
        else
        {
            Debug.DrawLine(pos_Tx, pos_Rx, Color.green);
        }

    }
}