/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>05th July 2017</date>
   <summary>
        Helper attatched to all Blockout prefabs for management and editor purpouses.
   </summary>*/

using System.Linq;
using UnityEngine;

namespace RadicalForge.Blockout
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
    public class BlockoutHelper : MonoBehaviour
    {
        public SectionID initialBlockoutSection;
        public bool ReapplyMaterialTheme = true;
        private bool reapplyMaterialThemeInternal = false;
        [SerializeField] private bool IsTriAsset = false;
        private bool valid = true;
        private bool locked = false;
        private Vector3 lockedPosition;
        private Quaternion lockedRotation;
        public bool Locked { get { return locked; } }
        private Mesh mesh;
        private Color[] colors;
        

        void Start()
        {

            if (Application.isEditor && !Application.isPlaying)
            {
                if (!reapplyMaterialThemeInternal)
                    reapplyMaterialThemeInternal = ReapplyMaterialTheme;
                else
                    ReapplyMaterialTheme = reapplyMaterialThemeInternal;

                if(transform.parent)
                {
                    if (transform.parent.GetComponent<BlockoutSection>())
                    return;
                }

                GameObject rootObject = GameObject.Find("Blockout");
                if (rootObject)
                {
                    var parents = rootObject.GetComponentsInChildren<BlockoutSection>()
                        .Where(x => x.transform.parent == rootObject.transform)
                        .Where(x => x.Section == initialBlockoutSection).Select(x => x.transform).ToArray();
                    if (parents.Length > 0)
                    {
                        Transform targetParent = parents.First();
                        if (targetParent)
                            transform.SetParent(targetParent);
                    }
                }
                else
                {
                    valid = false;
                }
            }
        }

        void Update()
        {
            if (valid)
            {
                if (!name.Contains(" (Tri-Planar)") && IsTriAsset)
                {
                    name += " (Tri-Planar)";
                    ReapplyMaterialTheme = true;
                }
            }

            if (locked)
            {
                transform.position = lockedPosition;
                transform.rotation = lockedRotation;
            }
        }

        public void SetLock(bool value)
        {
            Mesh original = GetComponent<MeshFilter>().sharedMesh;
            mesh = Instantiate(original);
            mesh.name = original.name;
            GetComponent<MeshFilter>().mesh = mesh;

            colors = new Color[mesh.vertices.Length];

            locked = value;
            if (value)
            {
                lockedPosition = transform.position;
                lockedRotation = transform.rotation;

                for (int i = 0; i < mesh.vertices.Length; i++)
                    colors[i] = Color.black;
            }
            else
            {
                for (int i = 0; i < mesh.vertices.Length; i++)
                    colors[i] = Color.red;
            }

            mesh.colors = colors;
        }

        public void HideLockedState()
        {
            if (locked)
            {
                Mesh original = GetComponent<MeshFilter>().sharedMesh;
                mesh = Instantiate(original);
                mesh.name = original.name;
                GetComponent<MeshFilter>().mesh = mesh;

                colors = new Color[mesh.vertices.Length];


                for (int i = 0; i < mesh.vertices.Length; i++)
                    colors[i] = Color.red;
                

                mesh.colors = colors;
            }
        }

        public void ShowLockedState()
        {
            if (locked)
            {
                Mesh original = GetComponent<MeshFilter>().sharedMesh;
                mesh = Instantiate(original);
                mesh.name = original.name;
                GetComponent<MeshFilter>().mesh = mesh;

                colors = new Color[mesh.vertices.Length];


                for (int i = 0; i < mesh.vertices.Length; i++)
                    colors[i] = Color.black;


                mesh.colors = colors;
            }
        }

    }

#else

    public class BlockoutHelper : MonoBehaviour
    {
        void Start()
        {
            Destroy(this);
        }
    }

#endif

}
