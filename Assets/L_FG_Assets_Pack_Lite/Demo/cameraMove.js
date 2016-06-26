var rSpeed = 3.0;
var mSpeed = 20.0;
var X = 0.0;
var Y = 0.0;

function Update (){

    X += Input.GetAxis("Mouse X")*rSpeed;    
    Y += Input.GetAxis("Mouse Y")*rSpeed; 
    transform.localRotation = Quaternion.AngleAxis(X, Vector3.up);
    transform.localRotation *= Quaternion.AngleAxis(Y, Vector3.left);
    transform.position += transform.forward*mSpeed*Input.GetAxis("Vertical")*Time.deltaTime;
    transform.position += transform.right*mSpeed*Input.GetAxis("Horizontal")*Time.deltaTime;
}
