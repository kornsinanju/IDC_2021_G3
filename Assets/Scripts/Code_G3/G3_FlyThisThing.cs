using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class G3_FlyThisThing : MonoBehaviour
{
    // Start is called before the first frame update
    public PhotonView photonView;
    private float tx, ty, tz, fx, fy, fz;
    private Rigidbody rb;
    // public PhotonView photonView;
    // private PhotonView photonView;
    // [SerializeField]
    // [Range(0,20)]
    private float Kp=1f,Ki=0.01f,Kd=0.001f;
    private float Kpx=22f,Kix=0.1f,Kdx=25f;
    private float Kpz=2.5f,Kiz=0.0f,Kdz=5f;
    private float Kpy=25f,Kiy=0.00f,Kdy=27f;
    private float targety=2,etx=0,etz=0,targetz=2,targetx=2,prev_etx=0,prev_etz=0;
    private float ex=0,ey=0,ez=0,prev_ex=0,prev_ey=0,prev_ez=0;
    private float pfx=0,ifx=0,dfx=0;
    private float pfy=0,ify=0,dfy=0;
    private float pfz=0,ifz=0,dfz=0;
    private float curx,cury,curz;
    private float ptx=0,itx=0,dtx=0;
    private float ptz=0,itz=0,dtz=0;
    private float initialy=0;
    private bool autopilot = false;
    private float limit = 100.0f;

    public float stability = 1.0f;
    public float speed = 3.0f;
    void Start()
    {

        rb=this.GetComponent<Rigidbody>();
        photonView = this.GetComponent<PhotonView>();
        targety = rb.position[1];
        targetx = rb.position[0];
        targetz = rb.position[2];
        // initialy = rb.position[1];
        // prevy = initialy;
        // targety = 0; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if(!photonView.IsMine)
            // return;

        fx=0.0f;
        fy=0.0f;
        fz=0.0f;
        tx=0.0f;
        ty=0.0f;
        tz=0.0f;

        if (photonView.IsMine)
        {
            if(Input.GetKey(KeyCode.K)){
                autopilot = false;
            }
            else if(Input.GetKey(KeyCode.O)){
                autopilot = true;
            }
            else if(Input.GetKey(KeyCode.I)){
                Kpx=220f;
                Kix=0.0f;
                Kdx=100f;
                Kpz=9f;Kiz=0.0f;
                Kdz=8f;
                Kpy=720f;Kiy=0.1f;
                Kdy=320f;
                autopilot = true;
            }
            if(!autopilot){
                if (Input.GetKey(KeyCode.A))
                {
                    // Debug.Log("pressed A");
                    fx= 20.0f;
                    targetx += 1.0f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    // Debug.Log("pressed D");
                    fx = -20.0f;
                    targetx -= 1.0f;
                }
                else 
                    fx = -rb.velocity[0]*10.0F;
                    if(fx > 100) fx=100f;
                    else if(fx < -100) fx=-100f;
                if (Input.GetKey(KeyCode.P))
                {
                    targety += 0.01F;
                    fy= 15.0f;
                }
                else if (Input.GetKey(KeyCode.L))
                {
                    targety -= 0.01F;
                    fy = -20.0f;
                }
                else 
                {
                    fy = -rb.velocity[1]*10.0F;
                    if(fy > 100) fy=100f;
                    else if(fy < -100) fy=-100f;
                }
                if (Input.GetKey(KeyCode.S))
                    fz= 20.0f;
                else if (Input.GetKey(KeyCode.W))
                    fz = -20.0f;
                else
                {
                    fz = -rb.velocity[2]*10.0F;
                    if(fz > 100) fz=100f;
                    else if(fz < -100) fz=-100f;
                }
                if (targety<initialy)
                    targety = initialy;

                if (Input.GetKey(KeyCode.E))
                    ty= 5.0f;
                else if (Input.GetKey(KeyCode.Q))
                    ty = -5.0f;
                else 
                {
                    ty = -rb.angularVelocity[1]*5.0F;
                    if(ty>20) ty=20f;
                    else if(ty<-20) ty=-20f;
                }
            }
            else{
                ty = -rb.angularVelocity[1]*5.0F;
                if(ty>20) ty=20f;
                else if(ty<-20) ty=-20f;
                // x rb.transform.position
                ex = 0.0f - rb.transform.position.x;
                pfx = ex;
                ifx += ex * 0.02f;
                dfx = (ex-prev_ex)/0.02f;
                prev_ex = ex;
                fx = pfx*Kpx + ifx*Kix + dfx*Kdx;
                if(fx > limit) fx = limit;
                if(fx < -limit) fx = -limit;

                ey = 18.7f - rb.transform.position.y;
                pfy = ey;
                ify += ey * 0.02f;
                dfy = (ey-prev_ey)/0.02f;
                prev_ey = ey;
                fy = pfy*Kpy + ify*Kiy + dfy*Kdy;
                if(fy > limit) fy = limit;
                if(fy < -limit) fy = -limit;

                ez = -12.2f - rb.transform.position.z;
                pfz = ez;
                ifz += ez * 0.02f;
                dfz = (ez-prev_ez)/0.02f;
                prev_ez = ez;
                fz = pfz*Kpz + ifz*Kiz + dfz*Kdz;
                if(fz > limit) fz = limit;
                if(fz < -limit) fz = -limit;
            }
            

        }
        
        //---------------------Hovering & Apply xyz force ----------------------
        
        Vector3 nf = new Vector3(0,3.75f * Mathf.Abs(Physics.gravity.y),0);
        rb.AddForce(nf);
        Vector3 f= new Vector3 (fx, fy, fz);
        Vector3 pos = rb.transform.position;
        Debug.Log(pos.ToString());
        //----------------------- Rotation control ---------------------
        Quaternion rot = rb.rotation;
        Vector3 ang = rb.angularVelocity;
        etx = 0.0f-rot[0];
        etz = 0.0f-rot[2];
        ptx = etx;
        ptz = etz;
        itx += etx*0.02f;
        itz += etz*0.02f;
        dtx = (etx-prev_etx)/0.02f;
        dtz = (etz-prev_etz)/0.02f;
        Debug.Log(etx.ToString() + " " + etz.ToString());
        prev_etx = etx;
        prev_etz = etz;

        tx = ptx*Kp + itx*Ki + dtx*Kd;
        tz = ptz*Kp + itz*Ki + dtz*Kd;
        //rb.AddRelativeForce(f);
        rb.AddForce(f);
        Vector3 t= new Vector3 (tx, ty, tz);
        // Debug.Log(ptx.ToString() + " " + ptz.ToString());
        // Debug.Log("Torque " + tx.ToString() + " " + tz.ToString());
        
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude*Mathf.Rad2Deg*stability/speed,rb.angularVelocity)*transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp,Vector3.up);
        Vector3 realtorque = torqueVector * speed * speed;
        if(realtorque.magnitude>20){
            realtorque.Normalize();
            realtorque = realtorque*20;
        }
        rb.AddTorque(realtorque);

        Vector3 tn= new Vector3 (0, ty, 0);
        // rb.AddRelativeTorque(t_new);
        rb.AddTorque(tn);
    }
}
