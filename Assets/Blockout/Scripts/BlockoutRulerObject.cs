#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BlockoutRulerObject : MonoBehaviour {

	LineRenderer lineRenderer;
	TextMesh distanceText;

	void OnEnable()
	{
		lineRenderer = GetComponent<LineRenderer>();
		distanceText = GetComponentInChildren<TextMesh>();
		gameObject.SetActive(!Application.isPlaying);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!distanceText.gameObject.activeInHierarchy)
	        return;

		for(int i = 0; i < 2; ++i)
			lineRenderer.SetPosition(i, transform.InverseTransformPoint(transform.GetChild(i).position));
		distanceText.text = string.Format("{0:F}", Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position));

        if(SceneView.lastActiveSceneView)
	        distanceText.transform.LookAt(SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f)));
	}
}

#else

using UnityEngine;

public class BlockoutRulerObject : MonoBehaviour {

	LineRenderer lineRenderer;
	TextMesh distanceText;

	void OnEnable()
	{
		lineRenderer = GetComponent<LineRenderer>();
		distanceText = GetComponentInChildren<TextMesh>();
		gameObject.SetActive(!Application.isPlaying);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!distanceText.gameObject.activeInHierarchy)
	        return;

		for(int i = 0; i < 2; ++i)
			lineRenderer.SetPosition(i, transform.InverseTransformPoint(transform.GetChild(i).position));
		distanceText.text = string.Format("{0:F}", Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position));
        
	    distanceText.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f)));
	}
}

#endif