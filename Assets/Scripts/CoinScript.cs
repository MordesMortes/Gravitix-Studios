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
    [HideInInspector]
    public GameObject CollidedObject;//object that has collided with the coin that is tagged with
    RealtimeAvatarManager _avatarManager;//gets all the avatars of the players
    private MeshRenderer myRend;//meshrenderer to make objects randomly colored and to to allow shaders to be changed


    private void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        myRend.material.SetColor("_BaseColor", Random.ColorHSV());
        _avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        Initialposition = gameObject.transform.position;
        InitialRotation = gameObject.transform.rotation;
        if (IsFake)
        {
            gameObject.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass + 0.5f;
        }
        
    }
    public void EnScale(float scale)
    {
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
    }
    public bool Reveal()
    {
        
        if (IsFake == false)
        {
            gameObject.transform.position = Initialposition;
            gameObject.transform.rotation = InitialRotation;
            foreach (var item in _avatarManager.avatars)
            {
                _avatarManager.avatars[item.Key].gameObject.GetComponent<GravityGun>().ReSpawn();
            }
            
            return false;
            
        }
        if (IsFake == true)
        {
            Destroy(gameObject, 3f);
            
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

    private void OnDestroy()
    {
        gameObject.GetComponentInParent<GravityGun>().DropObj();
    }

    public void Return()
    {
        gameObject.transform.position = Initialposition;
        gameObject.transform.rotation = InitialRotation;
        Shade("HDRP/Lit");
    }
    public void Shade(string Shade)
    {
        myRend.material.shader = Shader.Find(Shade);
    }
}
