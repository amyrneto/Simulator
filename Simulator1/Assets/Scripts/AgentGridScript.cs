using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGridScript : MonoBehaviour {

    public Transform gridPrefab;
    public int gridW, gridH;
    public int posX, posZ;
    public bool centered;

	// Use this for initialization
	public void BuildGrid () {
        for (int w = 0; w<gridW; w++)
        {
            for (int h=0; h<gridH; h++)
            {
                Vector3 pos = transform.position;
                float delta;
                if (centered)
                {
                    delta = -1.0f;
                } else
                {
                    delta = -0.5f;
                }
                pos.x = pos.x + ( (w-posX - delta) * Mathf.Ceil(gridPrefab.localScale.x) );
                pos.z = pos.z + ( (h-posZ - delta) * Mathf.Ceil(gridPrefab.localScale.z) );
                Transform t = Instantiate(gridPrefab, pos, Quaternion.identity);
                t.parent = this.transform;
            }
        }		
	}
}
