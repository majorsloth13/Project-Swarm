using UnityEngine;

public class IFrameState : IPlayerState
{
    [SerializeField] float IframeDurration;

    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public IFrameState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }
    public void Enter()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void Exit()
    {

    }
}
