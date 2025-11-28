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
    private ThrowBasketball throwBasketball;
    private NpcTriggerZone npcTriggerZone;
    private bool isFirstTimeTalk=true;
    private float cameraSwitchCooldown = 0.3f;
    private float lastSwitchTime = 0f;

    public void Start()
    {
        npcTriggerZone = NPCB1_B5.GetComponent<NpcTriggerZone>();
        HooperMoving = hooper.GetComponent<BK_HooperMoving>();
        cameraSwitcher = gameObject.GetComponent<PlayerCameraSwitcher>();
        throwBasketball = basketball.GetComponent<ThrowBasketball>();
        HooperMoving.OnMaxLevelReached += EndGame;
    }
    private void Update()
    {

        SwitchCamera();

    }

    public void StartGame()
    {
   
    }
    public void SwitchCamera()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame && !isFirstTimeTalk)
        {
            if (Time.time - lastSwitchTime < cameraSwitchCooldown)
                return;

            lastSwitchTime = Time.time;

            if (cameraSwitcher.IsFirstPersonView())
            {
                cameraSwitcher.SetFirstPerson(false);
                throwBasketball.ResetBallPosition();
            }
            else if (!cameraSwitcher.IsFirstPersonView() && npcTriggerZone.isPlayerInZone())
            {
                cameraSwitcher.SetFirstPerson(true);
                throwBasketball.ResetBallPosition();
            }
        }
    }

    public void EndGame()
    {
      
            cameraSwitcher.SetFirstPerson(false);
        throwBasketball.ResetBallPosition();
        GetReward();
        GameEventsManager.instance.questEvent.FinishQuest("Quest5Info");
        GameEventsManager.instance.questEvent.AdvanceQuest("Quest6Info");
    }

    public void GetReward()
    {
        UIManager_.Instance.Open<PopupMessage>().Show("B?n ?ã hoàn thành Quest, chúc m?ng, b?n v?a nh?n ???c Pcoin ", 1f);
    }
}
