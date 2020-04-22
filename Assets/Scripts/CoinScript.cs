using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Normal.Realtime;

public class CoinScript : MonoBehaviour
    
{
    [Range(1f,30f)]
    public int CoinWeight;//weight of this coin for the second puzzle
    public bool IsFake;//is this a real or fake coin for the first puzzle    
    Vector3 Initialposition;//initial position of gameobject
    Quaternion InitialRotation;//initial rotation of gameobject
    
    RealtimeAvatarManager _avatarManager;//gets all the avatars of the players
    private MeshRenderer myRend;//meshrenderer to make objects randomly colored and to to allow shaders to be changed
    public Material DefaultMaterial;
    public Material DefaultTransparent;//transparent version of the default material
    public Material Hologram;//hologram material to indicate realtime status
    public Material[] materials;
    private Color colour;//colour of the block so that it can be reapplied later


    private void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        myRend.material.SetColor("_BaseColor", Random.ColorHSV());
        colour = Random.ColorHSV();
        _avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        Initialposition = gameObject.transform.position;
        InitialRotation = gameObject.transform.rotation;
        if (IsFake)
        {
            gameObject.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass + 0.5f;
        }
        
    }
   
    public bool Reveal()
    {
        
        if (IsFake == false)
        {
            if (gameObject.GetComponentInParent<GravityGun>())
            {
                gameObject.GetComponentInParent<GravityGun>().DropObj();
                ReturnToStart();
            }
            if (gameObject.GetComponentInParent<GravityGun>() == false)
            {
                ReturnToStart();
                //ReturnToRealtime();
            }
            

            
            return false;
            
        }
        if (IsFake == true)
        {
            if (gameObject.GetComponentInParent<GravityGun>())
            {
                gameObject.GetComponentInParent<GravityGun>().DropObj();
                ReturnToStart();
            }
            if (gameObject.GetComponentInParent<GravityGun>() == false)
            {
                ReturnToStart();
                //ReturnToRealtime();
            }
            
           
            
            return true;
        }
        else 
        { 
            return false; 
        }
    }
        
     public void EnMass()
    {
        GetComponent<Rigidbody>().mass = CoinWeight;
    }

   
    public void ReturnToStart()
    {
        gameObject.transform.position = Initialposition;
        gameObject.transform.rotation = InitialRotation;
        
    }
    public void Shade()
    {
        materials[0] = DefaultTransparent;
        materials[1] = Hologram;
        myRend.materials = materials;
      
    }
    public void UnShade()
    {
        materials[0] = DefaultMaterial;
        materials[1] = DefaultMaterial;
        materials[1].SetColor("_BaseColor", colour);
        myRend.materials = materials;
        
    }
    public void ReturnToRealtime()
    {
        
        gameObject.GetComponent<RealtimeTransform>().enabled = true;
        gameObject.GetComponent<RealtimeView>().enabled = true;
        
    }
   
}
