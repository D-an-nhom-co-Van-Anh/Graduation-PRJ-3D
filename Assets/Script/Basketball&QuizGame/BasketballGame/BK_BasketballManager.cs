using UnityEngine;
using UnityEngine.InputSystem;

public class BasketballManager : MonoBehaviour
{
    [SerializeField]
    private GameObject NPCB1_B5 ;
    [SerializeField]
    private GameObject hooper;
    [SerializeField]
    private GameObject basketball;

    private BK_HooperMoving HooperMoving;
    private PlayerCameraSwitcher cameraSwitcher;
    private DialogueController dialogueController;
    private ThrowBasketball throwBasketball;
    private NpcTriggerZone npcTriggerZone;
    private bool isFirstTimeTalk=true;
    private float cameraSwitchCooldown = 0.3f;
    private float lastSwitchTime = 0f;

    public void Start()
    {
        npcTriggerZone = NPCB1_B5.GetComponent<NpcTriggerZone>();
        dialogueController=NPCB1_B5.GetComponent<DialogueController>();
        HooperMoving = hooper.GetComponent<BK_HooperMoving>();
        cameraSwitcher = gameObject.GetComponent<PlayerCameraSwitcher>();
        throwBasketball = basketball.GetComponent<ThrowBasketball>();
    }
    private void Update()
    {
        if (dialogueController.isFinishedDialogue() && !cameraSwitcher.IsFirstPersonView()&& isFirstTimeTalk)
        {
            isFirstTimeTalk = false;
            cameraSwitcher.SetFirstPerson(true);
        }
        if (Keyboard.current.vKey.wasPressedThisFrame && !isFirstTimeTalk  )
        {
            if (Time.time - lastSwitchTime < cameraSwitchCooldown)
                return; 

            lastSwitchTime = Time.time;  

            if (cameraSwitcher.IsFirstPersonView() )
            {
                cameraSwitcher.SetFirstPerson(false);
                throwBasketball.ResetBallPosition();
            }
            else if(!cameraSwitcher.IsFirstPersonView()  && npcTriggerZone.isPlayerInZone())
            {
                cameraSwitcher.SetFirstPerson(true);
                throwBasketball.ResetBallPosition();
            }
        }




        if (HooperMoving.GetLevel()==HooperMoving.MaxLevel())
        {
            EndGame();
        }
    }

    public void StartGame()
    {
   
    }

    public void EndGame()
    {
      
            cameraSwitcher.SetFirstPerson(false);
        throwBasketball.ResetBallPosition();
        GetReward(); 
    }

    public void GetReward()
    {
        UIManager_.Instance.Open<PopupMessage>().Show("B?n ?ã hoàn thành Quest, chúc m?ng, b?n v?a nh?n ???c Pcoin ", 1f);
    }
}
