using Godot;
using ProjectNostalgia.combat.hurtbox_component;
using ProjectNostalgia.events;
using ProjectNostalgia.skill;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.entity.player;

public partial class Player : Entity
{
    private const float Speed = 5.0f;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _stateMachine;
    private Camera3D _camera;
    private Vector3 _cameraOffset;
    private PlayerState _state;

    public override void _Ready()
    {
        base._Ready();
        _animationTree = GetNode("Model/AnimationTree") as AnimationTree;
        _stateMachine =
            (AnimationNodeStateMachinePlayback)((GetNode("Model/AnimationTree") as AnimationTree)!).Get(
                "parameters/playback");

        _camera = GetNode("Camera3D") as Camera3D;
        _cameraOffset = _camera!.Position;

        GetNode<HurtboxComponent>("HurtboxComponent").Init(CollisionUtils.OwnerType.Player);

        SetState(new MoveState(this));
        GD.Print("Components: ", GetManager().GetComponents().Count);
        Events.Instance.EmitSignal(Events.SignalName.PlayerSpawned, this);
        GD.Print("Player ready");
    }

    protected override void InitSkills()
    {
        foreach (Skill skill in Skills)
        {
            skill.Init(this, CollisionUtils.OwnerType.Player);
        }
    }

    private void ProcessCamera()
    {
        _camera.GlobalPosition = GlobalPosition + _cameraOffset;
        _camera.LookAt(GlobalPosition);
    }

    private abstract class PlayerState
    {
        protected Player MyPlayer;

        public PlayerState(Player myPlayer)
        {
            MyPlayer = myPlayer;
        }

        public abstract void OnStateEnter();
        public abstract void StateProcess(double delta);
        public abstract void OnStateExit();
    }

    private class MoveState : PlayerState
    {
        public MoveState(Player myPlayer) : base(myPlayer)
        {
        }

        public override void OnStateEnter()
        {
            // Cancels previous animation
            // MyPlayer._stateMachine.Travel("Idle");
        }

        public override void StateProcess(double delta)
        {
            if (Input.IsActionJustPressed("ui_attack") && MyPlayer.GetNode<Skill>("Skills/BasicAttack").CanUse())
            {
                // TODO: Wtf is this line 
                MyPlayer.SetState(new UseSkillState(MyPlayer, MyPlayer.GetNode("Skills/BasicAttack") as Skill));
                return;
            }

            Vector3 velocity = MyPlayer.Velocity;

            Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            Vector3 direction = (new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (direction != Vector3.Zero)
            { 
                if (MyPlayer._stateMachine.GetCurrentNode() != "Run") MyPlayer._stateMachine.Travel("Run");
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;
            }
            else
            {
                if (MyPlayer._stateMachine.GetCurrentNode() == "Run") MyPlayer._stateMachine.Travel("Idle");
                velocity.X = Mathf.MoveToward(MyPlayer.Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(MyPlayer.Velocity.Z, 0, Speed);
            }

            MyPlayer.Velocity = velocity;
            MyPlayer.RotateSmoothly(delta);
            
        }

        public override void OnStateExit()
        {
        }
    }

    private class UseSkillState : PlayerState
    {
        private Skill _skill;

        public UseSkillState(Player myPlayer, Skill skill) : base(myPlayer)
        {
            _skill = skill;
        }

        public override void OnStateEnter()
        {
            MyPlayer.Velocity = new Vector3(0, MyPlayer.Velocity.Y, 0);
            MyPlayer._stateMachine.Travel("Attack");
            _skill.Use();
            _skill.CastTimeFinished += _ExitState;

        }

        private void _ExitState()
        {
            MyPlayer.SetState(new MoveState(MyPlayer));
            _skill.CastTimeFinished -= _ExitState;
        }

        public override void StateProcess(double delta)
        {
        }

        public override void OnStateExit()
        {
        }
    }

    private void SetState(PlayerState newState)
    {
        _state?.OnStateExit();
        _state = newState;
        newState.OnStateEnter();
    }

    protected override void StartDying()
    {
        GD.Print("Player died");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _state.StateProcess(delta);
        ProcessCamera();
    }
}