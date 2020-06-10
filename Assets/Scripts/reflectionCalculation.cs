using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class reflectionCalculation : MonoBehaviour
{

    public int distanceMax;
    public Material Target_Material;

    Vector3[] Wall_vrtx = new Vector3[2];
    Vector3 target;
    Vector3 P_Tx;

    float perpDistance;
    
    RaycastHit hit;
    RaycastHit hitWall;
    RaycastHit hitReflect;
    RaycastHit reflect;
    RaycastHit hitLOS;
    RaycastHit hitCheck;

    public Vector3[] vrtx_world;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        foreach (Collider Building in wallsNearby())
        {
            calculate(Building);
            foreach (GameObject reciever in GameObject.FindGameObjectsWithTag("rx"))
            {
                reflectCalculate(reciever);
            }
        }
    }

    void calculate(Collider Building)
    {
        //int layerMask = 1 << 9;
        Mesh mesh = Building.GetComponent<MeshFilter>().mesh;
        Vector3[] vrtx = mesh.vertices;
        Vector3[] nrml = mesh.normals;
        float index;
        index = vrtx[3].y;
        int l = -1;
        Vector3[] Ground_vrtx = new Vector3[4] ;
        for (int i = 0; i < vrtx.Length; ++i)
        {
            if ((vrtx[i].y < 0.1) && (!Ground_vrtx.Contains(vrtx[i])))        //Some ground vrtx are not exactly .y=0
            {
               
                    ++l;
                    Ground_vrtx[l] = vrtx[i];          //Finding ground vertices
                
            }
            
        }



        Vector3[] Visble_Grnd_vrtx = new Vector3[4];
        Vector3[] Grnd_nrml = new Vector3[4];
        int m = 0;
        for (int i=0; i < Ground_vrtx.Length; ++i)
        {
            if(Physics.Raycast(transform.position, Ground_vrtx[i] - transform.position, out hitCheck) && (hitCheck.point== Ground_vrtx[i]))
            {
                Visble_Grnd_vrtx[m] = Ground_vrtx[i];
                Grnd_nrml[m] = hitCheck.normal;

                ++m;
            }
        }

        
        Vector3 wall_nrml;

        for (int i=0; i < Grnd_nrml.Length; ++i)
        {
            for (int j=0; j < 4; ++j )
            {
                if (Grnd_nrml[i] == Grnd_nrml[j] && j!=i)
                {
                    
                    Wall_vrtx[0] =  Visble_Grnd_vrtx[i];
                    Wall_vrtx[1] = Visble_Grnd_vrtx[j];
                    wall_nrml = Grnd_nrml[i];

                }

            }

        }

        if (Physics.Raycast(transform.position, Wall_vrtx[0] - transform.position, out hitWall, distanceMax))
        {
            Vector3 perpAngle = hitWall.normal;
            float hitDistance = hitWall.distance;
            //Vector3 hitDirection = Building.transform.position - transform.position;
            Vector3 hitDirection = hitWall.point - transform.position;
            float hitAngle = Vector3.Angle((hitWall.normal), -hitDirection.normalized);

            perpDistance = hitDistance * Mathf.Cos((hitAngle * Mathf.PI) / 180);

            target = transform.position - (hitWall.normal * perpDistance * 2);

            P_Tx = (transform.position + target) / 2;

            Vector3 Wall_vrlifted = Wall_vrtx[0];
            Wall_vrlifted.y = P_Tx.y;

            Vector3 V_vctr = (P_Tx - Wall_vrlifted).normalized;
        }



        //Math.Abs((Grnd_nrml[i] - Grnd_nrml[j]).magnitude) < 0.1


        //if (Physics.Raycast(transform.position, Building.transform.position - transform.position, out hitWall, distanceMax, layerMask))
        //if (Physics.Raycast(transform.position, Building.bounds.center - transform.position, out hitWall, distanceMax, layerMask))
        /* {
             Mesh mesh = hitWall.collider.GetComponent<MeshFilter>().mesh;
             print(mesh.isReadable);
             Vector3[] vrtx = mesh.vertices;
             Vector3[] nrml = mesh.normals;

             for (int i = 0; i < vrtx.Length; ++i)
             {
                 vrtx_world[i] = hitWall.collider.transform.TransformPoint(vrtx[i]);
             }
             //   Vector3[] vrtx = Building.GetComponent<MeshFilter>().mesh.vertices;
             //  Vector3[] nrml = Building.GetComponent<MeshFilter>().mesh.normals;

             Vector3 perpAngle = hitWall.normal;
             float hitDistance = hitWall.distance;
             //Vector3 hitDirection = Building.transform.position - transform.position;
             Vector3 hitDirection = hitWall.point - transform.position;
             float hitAngle = Vector3.Angle((hitWall.normal), -hitDirection.normalized);

             perpDistance = hitDistance * Mathf.Cos((hitAngle * Mathf.PI) / 180);

             target = transform.position - (hitWall.normal * perpDistance * 2);

             GameObject v_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
             v_sphere.transform.position = target;
             Destroy(v_sphere.GetComponent<SphereCollider>()); // remove collider
             var sphereRenderer = v_sphere.GetComponent<Renderer>();
             sphereRenderer.material = Target_Material;
         }*/
    }

    void reflectCalculate(GameObject reciever)
    {
        if (Physics.Raycast(reciever.transform.position, Wall_vrtx[0] - reciever.transform.position, out hit, distanceMax))
        {
            if (Physics.Raycast(transform.position, hit.point - transform.position, out hitReflect, distanceMax) && (hit.point==hitReflect.point))
            //if ( hit.point == hitReflect.point )
            {
                //Debug.DrawLine(transform.position, transform.position - (hitWall.normal * perpDistance * 2), Color.red);
                //Debug.DrawLine(reciever.transform.position, target, Color.red);
                Debug.DrawLine(transform.position, hit.point, Color.green);
                Debug.DrawLine(hit.point, reciever.transform.position, Color.green);
                
                //Debug.DrawLine(transform.position, reciever.transform.position, Color.yellow);
            }
        }
        Debug.DrawLine(transform.position, reciever.transform.position, Color.yellow);
        if (Physics.Raycast(transform.position, reciever.transform.position - transform.position, out hitLOS))// && ((hitLOS.point - reciever.transform.position).magnitude < 10) )
        {
            //Debug.Log("!!!!![" + (hitLOS.point - reciever.transform.position).magnitude);
            Vector3 ant2obs = hitLOS.point - transform.position;
            Vector3 ant2ant = reciever.transform.position - transform.position;

            Debug.DrawLine(transform.position, reciever.transform.position, Color.yellow);

            if ( ant2obs.magnitude > ant2ant.magnitude+3)
            {
                Debug.DrawLine(transform.position, reciever.transform.position, Color.yellow);
                //Debug.Log("ant2obs = " +ant2obs.magnitude + "; ant2ant = " +ant2ant.magnitude);
            }
            else
            {
                Debug.DrawLine(transform.position, reciever.transform.position, Color.red);
            }
        }
    }


    private Collider[] wallsNearby()
    {
        int layerMask = 1 << 9;

        //List<Vector3> positions = new List<Vector3>();

        Collider[] walls = Physics.OverlapSphere(transform.position, distanceMax, layerMask);
        /*for (int i = 0; i < walls.Length; i++)
        {
            Vector3 temp_position = walls[i].bounds.center;
            positions.Add(temp_position);
        }*/


        return walls;
    }
}
