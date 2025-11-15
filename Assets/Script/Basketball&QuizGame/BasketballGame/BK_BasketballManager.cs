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
    
    
    public void Start()
    {
        dialogueController=NPCB1_B5.GetComponent<DialogueController>();
        HooperMoving = hooper.GetComponent<BK_HooperMoving>();
        cameraSwitcher = gameObject.GetComponent<PlayerCameraSwitcher>();
        throwBasketball = basketball.GetComponent<ThrowBasketball>();
    }
    private void Update()
    {
        if (dialogueController.isFinishedDialogue() && !cameraSwitcher.IsFirstPersonView())
        {
            cameraSwitcher.SetFirstPerson(true);
        }
        if (Keyboard.current.vKey.wasPressedThisFrame && cameraSwitcher.IsFirstPersonView())
        {
            cameraSwitcher.SetFirstPerson(false);
            if (throwBasketball != null)
                throwBasketball.ResetBallPosition();
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
      
        if (cameraSwitcher != null)
            cameraSwitcher.SetFirstPerson(false);
        throwBasketball.ResetBallPosition();
        GetReward(); 
    }

    public void GetReward()
    {
        UIManager_.Instance.Open<PopupMessage>().Show("B?n ?ã hoàn thành Quest, chúc m?ng, b?n v?a nh?n ???c Pcoin ", 1f);
    }
}
