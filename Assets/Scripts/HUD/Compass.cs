//a compass that shows the player where check points and home are 
using UnityEngine;

public class Compass : MonoBehaviour
{
    public GameObject stevePosition; //find Steve, is set up in the editor
    public GameObject compassTarget; //find current target, is set up in the editor
    public GameObject compassPointer; //find compass, is set up in the editor
    
    public RectTransform compassLine; //find rect transform of compass bar, is set up in the editor
    private RectTransform compassCalculation; //rect transform of the compass


    void Start()
    {
        compassCalculation = compassPointer.GetComponent<RectTransform>(); //get the rect transform
    }

    void Update()
    {
        //calculate, find and display where check points and home are to player if the game is not over or Steve hasn't fainted
        if (!GameStats.hasFainted || !GameStats.isDead || !GameStats.wonGame)
        {
            //don't do calculations if Steve is no longer in there
            if(stevePosition == null)
            {
                return;
            }

            Vector3[] corners = new Vector3[4];
            compassLine.GetLocalCorners(corners);

            float scale = Vector3.Distance(corners[1], corners[2]);

            Vector3 location = compassTarget.transform.position - stevePosition.transform.localPosition;

            float angle = Vector3.SignedAngle(stevePosition.transform.forward, location, stevePosition.transform.up);
            angle = Mathf.Clamp(angle, -90, 90) / 180.0f * scale;

            compassCalculation.localPosition = new Vector3(angle, compassCalculation.localPosition.y, compassCalculation.localPosition.z);
        }
    }
}
