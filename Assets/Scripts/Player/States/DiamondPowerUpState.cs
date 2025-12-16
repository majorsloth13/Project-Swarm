using UnityEngine;

public class DiamondPowerUpState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public DiamondPowerUpState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }
    public void Enter()
    {
        machine.diamondSkinActive = true;
        
        machine.diamondSkinCurrentHealth = machine.diamondSkinMaxHealth;
    }

    // Update is called once per frame
    public void Update()
    {
        if (machine.diamondSkinCurrentHealth <= 0)
        {
            Debug.Log("Diamond Skin Broke!");
            machine.SwitchState(machine.previousState); // restore last state
        }

        //if (!machine.IsGrounded)
        //{

        //    machine.SwitchState(new FallState(machine));
        //    return;
        //}

        //float input = Input.GetAxisRaw("Horizontal");
        //machine.FlipToGunDirection();
        //if (Mathf.Abs(input) > 0.01f)
        //{
        //    machine.SwitchState(new RunState(machine));
        //    return;
        //}

        //if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        //{
        //    Debug.Log("got jump from idel");
        //    machine.SwitchState(new JumpState(machine));
        //    return;
        //}
        //if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        //{
        //    Debug.Log("double jump!");
        //    machine.SwitchState(new DoubleJumpState(machine));
        //    return;
        //}
    }

    public void Exit()
    {
        machine.diamondSkinActive = false;
        Debug.Log("exited diamond skin state");
        
    }
}
