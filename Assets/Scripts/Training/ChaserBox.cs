using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBox : MonoBehaviour
{
    public Ship agentShip;
    public Ship enemyShip;
    Vector3 agentStartPos;
    Vector3 enemyStartPos;
    

    private void Awake()
    {
        agentStartPos = agentShip.transform.position;
        enemyStartPos = enemyShip.transform.position;

    }
    public void ResetScene()
    { 
        TrainingController.Instance.LogTrainingAttempt();
        agentShip.transform.position = agentStartPos;
        agentShip.transform.rotation = Quaternion.identity;

        Rigidbody2D rb = agentShip.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        Rigidbody2D rbsub = enemyShip.GetComponent<Rigidbody2D>();
        rbsub.velocity = Vector3.zero;
        rbsub.angularVelocity = 0;

        UIController.Instance.bearingInputField.text = "0";
        UIController.Instance.trainingEngineInputField.text = "0";

        ChaserAgent agent = agentShip.GetComponent<ChaserAgent>();
        enemyShip.GetComponent<BoxCollider2D>().size = new Vector2(agent.uboatBoxSize, agent.uboatBoxSize);

        float xPos = Random.Range(-agent.uboatMaxPlacementRange, agent.uboatMaxPlacementRange);
        float yPos = Random.Range(-agent.uboatMaxPlacementRange, agent.uboatMaxPlacementRange);
        if (xPos < 1 && xPos > -1)
            xPos *= 4;
        if (yPos < 1 && yPos > -1)
            yPos *= 4;
        enemyShip.transform.localPosition = new Vector3(xPos, yPos, enemyShip.transform.localPosition.z);
        if (Random.Range(0, 2) == 0)
            enemyShip.transform.GetComponent<Uboat>().Resurface();
        else
            enemyShip.transform.GetComponent<Uboat>().Resurface();
    }
    public void CircleUboat()
    {

    }
}
