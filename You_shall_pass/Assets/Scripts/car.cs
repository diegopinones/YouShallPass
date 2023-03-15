using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class car : MonoBehaviour
{
    public string parent = "Cars";
    public follow_target follow_target;
    

    //Velocity
    public float targetVelocity;
    public float velocity = 0;

    //Acceleration
    public float acceleration;
    public float initialAcceleration;
    private float initialTargetVelocity;

    //Steer
    public bool steer;

    //Front object detection
    public string frontObject = "";
    public car frontCar;

    //Front raycast
    public float rayStart;
    public float rayDistance;

    //intersection
    public bool hasStopped = false;
    public bool crossing = false;
    public bool onIntersection = false;
    public intersection intersection;
    public bool assignedDirection = false;
    public int directionInt = 0;
    public float crossTimer = 0;
    public GameObject animatorObject;
    private Animator animator;
    public int startTurning;

    //trigger stay check
    private bool triggerStay = false;

    //centro de calle actual
    [SerializeField] private AnimationCurve centerCurve;
    private float centerTimer;
    public bool startCentering;
    Transform laneCenter;
    string axis;
    float side = 0;
    float targetSide = 0;


    void Start()
    {
        this.transform.SetParent(GameObject.Find(parent).transform);
        initialAcceleration = acceleration;
        initialTargetVelocity = targetVelocity;
        velocity = targetVelocity;
        animator = animatorObject.GetComponent<Animator>();
        follow_target = GameObject.Find("Main Camera").GetComponent<follow_target>();
        centerTimer = 0;

        startCentering = false;
        startTurning = 0;
        frontCar = null;
    }

    void Update()
    {
        //if not crossing intersection
        if (!crossing)
        {
            //condiciones normales, avanzar, no avanzar
            //Debug.Log("normal");
            crossTimer = 0;
            //startTurning = false;
            if (steer)
            {
                Steer();
            }
            else
            {
                Brake();
            }

            //centrarse en carril
            if (laneCenter == null)
            {
                RaycastHit hitR;
                Physics.Raycast(transform.position - transform.up * 1f, -transform.up * 8f, out hitR, 10);

                if (hitR.transform != null)
                {
                    centerTimer = 0;
                    float rotY = transform.rotation.eulerAngles.y;

                    //estandarizar rotacion entre -179 a 180
                    while (rotY > 180)
                    {
                        rotY -= 360;
                    }

                    while (rotY <= -180)
                    {
                        rotY += 360;
                    }


                    //identificar que axis es el del lado
                    if ((rotY > -30 && rotY < 30) || (rotY > 150 || rotY < -150))
                    {
                        axis = "X";
                    }
                    else if ((rotY > 60 && rotY < 120) || (rotY > -120 && rotY < -60))
                    {
                        axis = "Z";
                    }

                    if (hitR.collider.tag == "lane center")
                    {
                        laneCenter = hitR.transform;
                    }
                }
            }
            else
            {
                if (axis == "X")
                {
                    
                    if (Mathf.Abs(transform.position.x - laneCenter.position.x) > 0.001f)
                    {

                        //solo centrarse al avanzar
                        if (velocity > .3f)
                        {
                            centerTimer += Time.deltaTime;
                        }

                        //inicio
                        if (!startCentering)
                        {
                            side = transform.position.x;
                            targetSide = laneCenter.position.x;
                            startCentering = true;
                        }
                        else
                        {
                            //ejecucion
                            float newSide = side + centerCurve.Evaluate(centerTimer / 2) * (targetSide - side);
                            transform.position = new Vector3(newSide, transform.position.y, transform.position.z);
                        }
                        if (centerTimer / 2 >= 1)
                        {
                            centerTimer = 0;
                            startCentering = true;
                        }
                            
                    }
                    else
                    {
                        centerTimer = 0;
                        startCentering = true;
                    }

                }
                else if (axis == "Z")
                {
                    //Debug.Log(centerTimer);
                    if (Mathf.Abs(transform.position.z - laneCenter.position.z) > 0.001f)
                    {
                        //solo centrarse al avanzar
                        if (velocity > .3f)
                        {
                            centerTimer += Time.deltaTime;
                        }

                        //inicio
                        if (!startCentering)
                        {
                            side = transform.position.z;
                            targetSide = laneCenter.position.z;
                            startCentering = true;
                        }
                        else
                        {
                            //ejecucion
                            float newSide = side + centerCurve.Evaluate(centerTimer / 2) * (targetSide - side);
                            transform.position = new Vector3(transform.position.x, transform.position.y, newSide);
                        }
                        if (centerTimer / 2 >= 1)
                        {
                            centerTimer = 0;
                            startCentering = false;
                        }
                    }
                }
                else
                {
                    centerTimer = 0;
                    startCentering = false;
                }
            }
        
    }
        else if (crossing && assignedDirection) {
            //si esta cruzando una interseccion

            //tiempo cruzando
            crossTimer += Time.deltaTime;

            //condiciones dependiendo de direccion
            if (intersection.getDirection(directionInt) == "Front"){
                //si se va para el frente 

                //avanzar a velocidad normal
                acceleration = initialAcceleration;
                targetVelocity = initialTargetVelocity;
                Steer();

                //despues del tiempo, resetear variables de cruce
                if (crossTimer >= 1f) {
                    crossing = false;
                    assignedDirection = false;
                    intersection = null;
                    onIntersection = false;
                    steer = true;
                }
            }
            else if (intersection.getDirection(directionInt) == "Right"){
                //si da vuelta

                //en primeros 3 frames asignar variables necesarias para dar vuelta
                if(startTurning < 3){ 
                    animatorObject.transform.SetParent(this.transform.parent, true);
                    transform.SetParent(animatorObject.transform, true);
                    //transform.parent.GetComponent<Animator>().applyRootMotion = false;
                    animator.enabled = true;
                    //Debug.Log("bool a true");
                    animator.SetBool("turn", true);
                    //this.gameObject.layer = 2;

                    startTurning++;
                }
                
                //cuando se acaba la animacion (el animator cambia el bool al acabar animacion)
                if (animator.GetBool("turn") == false) {
                    //volver a condiciones normales
                    //Debug.Log("acabo vuelta");
                    transform.SetParent(this.transform.parent.parent, true);
                    animatorObject.transform.SetParent(transform, true);
                    
                    velocity = targetVelocity;
                    //transform.parent.GetComponent<Animator>().applyRootMotion = true;
                    //this.gameObject.layer = 0;
                    animator.enabled = false;
                    crossing = false;
                    assignedDirection = false;
                    intersection = null;
                    onIntersection = false;
                    steer = true;

                    laneCenter = null;
                    axis = "";

                    startTurning = 0;
                    Debug.Log("Stopped turning");
                }
            }
        }
        if (crossing)
        {
            startCentering = false;
        }

        //Raycast corto de frente para acomodarse en interseccion
        Debug.DrawRay(transform.position + transform.forward * rayStart, transform.forward * rayDistance);
    }

    void LateUpdate() {
        //resetear variable que checa si se corre on trigger stay
        triggerStay = false;
    }
    void FixedUpdate() {
        //en condiciones normales
        if (!crossing) {
            //resetear deteccion de objecto del frente (quita bug de tener 2 obj dif en triggerstay)
            frontObject = "";

            //deteccion del frente
            RaycastHit frontRCH;
            if(Physics.Raycast(transform.position + transform.forward * rayStart, transform.forward, out frontRCH, rayDistance * 7)) {
                frontObject = frontRCH.transform.gameObject.tag;
            }
            //si no hay nada en el frente avanza y quitar estado de acomodo (quita bug de no alcanzar a frenar y pasarse de la interseccion y no quitar el hasstopped)
            if (frontObject == "") {
                steer = true;
                hasStopped = false;
            }

            //si hasStopped (acercarse despues de frenado en semaforo)
            if(hasStopped) {
                //maneja mas lento
                targetVelocity = initialTargetVelocity / 2;

                //acelera mas lento y frena mas rapido
                if(steer) {
                    acceleration = initialAcceleration / 2;
                }
                else if(!steer) {
                    acceleration = initialTargetVelocity * 2;
                }
            }
            else {
                //en estado normal, aceleracion y velocidad normal
                acceleration = initialAcceleration;
                targetVelocity = initialTargetVelocity;
            }

            //si no hay nada en trigger (quita bug de cuando el frontObject existe y no esta colisionando con nada)
            if (!triggerStay) {
                steer = true;
            }
        }
        if (crossing)
        {
            hasStopped = false;
        }
    }

    void Steer() {
        //cambiar gradualmente la variable velocidad
        if (velocity < targetVelocity) {
            velocity += acceleration * Time.deltaTime;
        }
        else {
            velocity = targetVelocity; 
        }
        //moverlo dependiendo de la velocidad
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);
    }
    void Brake(){
        //cambiar gradualmente hasta 0
        if (velocity > 0) {
            velocity -= acceleration * Time.deltaTime;
        }
        else {
            velocity = 0; 
        }
        //moverlo dependiendo de la velocidad
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);
    }

    private void OnTriggerStay(Collider other) {
        //registrar que corre triggerStay
        triggerStay = true;

        //si no cruza
        if(!crossing){
            //detectar si es algun trigger que detenga
            bool stopTrigger = false;


            //si es carro
            if (other.gameObject.tag == "car" && frontObject == "car") {
                if (frontCar == null) {
                    frontCar = other.gameObject.GetComponent<car>();
                }
                if (frontCar.velocity > frontCar.targetVelocity*.8f)
                {
                    if (this.targetVelocity > frontCar.targetVelocity)
                    {
                        steer = true;
                        targetVelocity = frontCar.targetVelocity;
                    }
                    else
                    {
                        targetVelocity = initialTargetVelocity;
                        steer = true;
                    }
                }
                else
                {
                    steer = false;
                    stopTrigger = true;
                    targetVelocity = initialTargetVelocity;
                }
                
            }
            else
            {
                frontCar = null;
            }

            //si es interseccion
            if (other.gameObject.tag == "intersection" && frontObject == "intersection") {
                steer = false;
                stopTrigger = true;

                //conseguir interseccion 
                intersection = other.gameObject.GetComponent<intersection>();
                onIntersection = true;
                //decidir direccion
                if (!assignedDirection) {
                    directionInt = intersection.assignDirection();
                    assignedDirection = true;
                    Debug.Log(intersection.getDirection(directionInt));
                }
                else {
                    //cruzar hasta que este disponible
                    if (intersection.getDirection(directionInt) == "Right") {
                        //despues de bajar la velocidad y acercarse lo mas posible a interseccion, checar si esta disponible la direccion
                        RaycastHit crossingHit;
                        if (intersection.Available(directionInt) && velocity < 0.3f
                            && (Physics.Raycast(transform.position + transform.forward * rayStart, transform.forward, out crossingHit, rayDistance))){
                            //entrar en estado de cruce
                            crossing = true;
                        }
                    }
                    else if (intersection.getDirection(directionInt) == "Front") {
                        //si se puede ir al frente
                        if (intersection.Available(directionInt)) {
                            //entrar en estado de cruce
                            crossing = true;
                        }
                    }
                    
                }
            }

            //cuando se detiene, hacer raycast para acercarse lo mas posible
            if ((velocity == 0 || hasStopped) && stopTrigger) {
                //Debug.Log("Acomodandose");
                hasStopped = true;
                targetVelocity = initialTargetVelocity / 2;
                //raycast hacia el frente, si se detecta, frenar con doble de aceleracion
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.forward * rayStart, transform.forward, out hit, rayDistance) 
                && (hit.transform.gameObject.tag == "car" || hit.transform.gameObject.tag == "intersection")) {
                    acceleration = initialAcceleration * 2;
                    steer = false;
                    //Debug.Log("Raycast hit");
                }
                else {
                    acceleration = initialAcceleration / 2;
                    steer = true;
                }
            }
            else {

            }
        }
    }
    private void OnTriggerExit(Collider other) {
        //al salirse el carro del frente del trigger, resetear variables
        if (other.gameObject.tag == "car" && frontObject == "car") {
            steer = true;
            hasStopped = false;
        }
    }

    private void OnCollisionEnter(Collision other) {
        //al chocar
        if (other.gameObject.tag == "car") {
            game_controller.crashes++;
            //cambiar variables de rigidbody y desabilidar ambos
            //Debug.Log("crash");
            this.gameObject.GetComponent<Rigidbody>().drag = .5f;
            this.gameObject.GetComponent<Rigidbody>().angularDrag = 0.5f;
            animatorObject.GetComponent<Animator>().enabled = false;
            if (game_controller.lives <= game_controller.crashes)
            {
                follow_target.target.position = transform.position + new Vector3(.8f, 0, 2f);
                follow_target.timeTarget = 1;
                Debug.Log("camera target moved");
            }
            this.enabled = false;
        }
    }
}
