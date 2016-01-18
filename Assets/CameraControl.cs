using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float Speed = 1;
    public Vector3 lookAtpos = Vector3.up;
    public PartEditor PE;
	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
        lookAtpos = lookAtpos + (Vector3.up * Speed * Input.GetAxis("Vertical") * Time.deltaTime);
        transform.position = transform.position + (Vector3.up * Speed * Input.GetAxis("Vertical") * Time.deltaTime);
        transform.position = transform.position + (transform.forward * Speed * Input.GetAxis("Zoom") * Time.deltaTime);

        Vector3 cross = Vector3.Cross(Vector3.up, transform.forward);
        transform.position = transform.position + (cross * Speed * Input.GetAxis("Horizontal") * Time.deltaTime);

        transform.LookAt(lookAtpos);

        if (Input.GetMouseButton(0))
        {
            Vector2 mDelta = Input.mouseScrollDelta;
            PE.Piece.transform.position = PE.Piece.transform.position + (cross * mDelta.x) + (Vector3.up * mDelta.y);
            PE.UpdateTransformValues();
        }

    }
}
