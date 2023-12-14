using System;
using Godot;
using ProjectNostalgia.combat.hitbox_component;
using ProjectNostalgia.combat.hurtbox_component;
using ProjectNostalgia.combat.loot_dropper_component;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.skill;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.entity.monster;

public partial class Monster : Entity
{
    [Export] private float _leashRange = 5.0f;
    [Export] private float _baseMaxHealth = 10;
    [Export] private float _baseDamage = 5;
    [Export] private float _baseMovementSpeed = 3;

    private int _level;
    
    private NavigationAgent3D _navigationAgent3D;
    private HurtboxComponent _player;
    private MonsterState _state;
    private AnimationNodeStateMachinePlayback _stateMachine;
    private AnimationTree _animationTree;
    private Node3D _model;
    private HitboxComponent _playerDetectionHitbox;
    private LootDropperComponent _lootDropper;

    public override void _Ready()
    {
        base._Ready();
        _model = GetNode("Model") as Node3D;
        _navigationAgent3D = GetNode("NavigationAgent3D") as NavigationAgent3D;
        _stateMachine =
            (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("Model/AnimationTree").Get(
                "parameters/playback");
        _animationTree = GetNode("Model/AnimationTree") as AnimationTree;
        _playerDetectionHitbox = GetNode<HitboxComponent>("PlayerDetectionHitbox");
        _lootDropper = GetNode<LootDropperComponent>("LootDropperComponent");

        GetNode<HurtboxComponent>("HurtboxComponent").Init(CollisionUtils.OwnerType.Monster);
        _playerDetectionHitbox.Init(CollisionUtils.OwnerType.Monster);
        ChangeState(new IdleState(this));
    }

    public static Monster CreateMonsterAndAddChild(PackedScene monsterScene, Node parent, int level)
    {
        Monster monster = monsterScene.Instantiate<Monster>();
        parent.AddChild(monster);
        monster.Init(level);
        return monster;
    }
    
    private void Init(int level)
    {
        _level = level;
        CalculateBaseStats();
    }

    private float GetLevelMultiplier()
    {
        return (float)(_level + Math.Pow(1.5, 0.2 * _level) + 1);
    }
    
    private void CalculateBaseStats()
    {
        float maxHp = (float)Math.Round(_baseMaxHealth * GetLevelMultiplier());
        float physicalDamage = (float)Math.Round(_baseDamage * GetLevelMultiplier());

        StatsComponent statsComponent = GetManager().GetComponent<StatsComponent>();
        statsComponent.SetStatBaseValue("MaxHP", maxHp);
        statsComponent.SetStatBaseValue("PhysicalDamage", physicalDamage);
        statsComponent.SetStatBaseValue("MovementSpeed", _baseMovementSpeed);
    }

    protected override void InitSkills()
    {
        foreach (Skill skill in Skills)
        {
            skill.Init(this, CollisionUtils.OwnerType.Monster);
        }
    }

    private abstract class MonsterState
    {
        protected Monster MyMonster;

        public MonsterState(Monster myMonster)
        {
            MyMonster = myMonster;
        }

        public abstract void OnStateEnter();
        public abstract void StateProcess(double delta);
        public abstract void OnStateExit();
    }

    // TODO: Make the monster move around a little bit (?)
    private class IdleState : MonsterState
    {
        public IdleState(Monster myMonster) : base(myMonster)
        {
        }

        public override void OnStateEnter()
        {
            MyMonster.Velocity = new Vector3(0, MyMonster.Velocity.Y, 0);
            MyMonster._stateMachine.Travel("Idle");
        }

        public override void StateProcess(double delta)
        {
            if (MyMonster._player != null)
            {
                {
                    MyMonster.ChangeState(new FollowState(MyMonster));
                }
            }
        }

        public override void OnStateExit()
        {
        }
    }

    // TODO: On entry make sure an exclamation mark is shown, the enemy turns, pauses for a bit and only then starts moving.
    private class FollowState : MonsterState
    {
        public FollowState(Monster myMonster) : base(myMonster)
        {
        }

        public override void OnStateEnter()
        {
            MyMonster._stateMachine.Travel("Run");
        }

        public override void StateProcess(double delta)
        {
            if (MyMonster.GlobalPosition.DistanceTo(MyMonster._player.GlobalPosition) >= MyMonster._leashRange)
            {
                MyMonster._player = null;
                MyMonster.ChangeState(new IdleState(MyMonster));
                return;
            }

            Skill skill = MyMonster.CanUseSkill();
            if (skill != null)
            {
                MyMonster.ChangeState(new AttackState(MyMonster, skill));
                return;
            }

            MyMonster._navigationAgent3D.TargetPosition = MyMonster._player.GlobalPosition;
            Vector3 velocity = MyMonster.Velocity;

            if (MyMonster._navigationAgent3D.IsTargetReachable())
            {
                Vector3 nextLocation = MyMonster._navigationAgent3D.GetNextPathPosition();
                Vector3 direction = MyMonster.GlobalPosition.DirectionTo(nextLocation);
                velocity.X = direction.X * MyMonster.GetStat("MovementSpeed");
                velocity.Z = direction.Z * MyMonster.GetStat("MovementSpeed");
            }
            else
            {
                velocity = Vector3.Zero;
            }

            MyMonster.Velocity = velocity;
            MyMonster.RotateSmoothly(delta);
        }

        public override void OnStateExit()
        {
        }
    }

    private class AttackState : MonsterState
    {
        private Skill _skill;

        public AttackState(Monster myMonster, Skill skill) : base(myMonster)
        {
            _skill = skill;
        }

        private void _ExitAttackState(StringName animationName)
        {
            MyMonster.ChangeState(new IdleState(MyMonster));
        }

        public override void OnStateEnter()
        {
            MyMonster.Velocity = new Vector3(0, MyMonster.Velocity.Y, 0);
            _skill.Use();
            MyMonster._stateMachine.Travel("Attack");
            MyMonster._animationTree.AnimationFinished += _ExitAttackState;
        }

        public override void StateProcess(double delta)
        {
        }

        public override void OnStateExit()
        {
            _skill.Cancel();
            MyMonster._animationTree.AnimationFinished -= _ExitAttackState;
        }
    }

    private class DyingState : MonsterState
    {
        public DyingState(Monster myMonster) : base(myMonster)
        {
        }

        private void _ExitDyingState(StringName animationName)
        {
            MyMonster._animationTree.AnimationFinished -= _ExitDyingState;
            if (animationName != "Die") return;
            MyMonster.ChangeState(new DespawningState(MyMonster));
        }

        public override void OnStateEnter()
        {
            CollisionShape3D collisionShape = MyMonster.GetNode("CollisionShape3D") as CollisionShape3D;
            MyMonster.SetCollisionLayerValue(CollisionUtils.GetLayer(CollisionUtils.CollisionLayer.MonsterCollision), false);
            MyMonster.SetCollisionMaskValue(CollisionUtils.GetLayer(CollisionUtils.CollisionLayer.PlayerCollision), false);
            MyMonster.SetCollisionMaskValue(CollisionUtils.GetLayer(CollisionUtils.CollisionLayer.MonsterCollision), false);
            MyMonster.SetCollisionMaskValue(CollisionUtils.GetLayer(CollisionUtils.CollisionLayer.DestructibleCollision), false);
            MyMonster._stateMachine.Travel("Die");
            MyMonster._animationTree.AnimationFinished += _ExitDyingState;
            MyMonster._lootDropper.DropLoot(MyMonster._level); // TODO: change level to match monster's level
        }

        public override void StateProcess(double delta)
        {
        }

        public override void OnStateExit()
        {
        }
    }

    private class DespawningState : MonsterState
    {
        public DespawningState(Monster myMonster) : base(myMonster)
        {
        }

        private void _OnTimerTimeout()
        {
            MyMonster.QueueFree();
        }

        public override void OnStateEnter()
        {
            MyMonster._animationTree.Active = false;
            Timer timer = new Timer();
            timer.WaitTime = 5;
            timer.Timeout += _OnTimerTimeout;
            MyMonster.AddChild(timer);
            timer.Start();
        }

        public override void StateProcess(double delta)
        {
            MyMonster._model.Position -= new Vector3(0, 0.3f * (float)delta, 0);
        }

        public override void OnStateExit()
        {
        }
    }

    private void ChangeState(MonsterState newState)
    {
        _state?.OnStateExit();
        _state = newState;
        newState.OnStateEnter();
    }

    private Skill CanUseSkill()
    {
        foreach (Skill skill in Skills)
        {
            if (skill.CanUse() && skill.HasTarget()) return skill;
        }

        return null;
    }

    protected override void StartDying()
    {
        GD.Print("Monster died");
        ChangeState(new DyingState(this));
    }

    private void _OnTargetFound(HurtboxComponent target)
    {
        _player = target;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _state.StateProcess(delta);
    }
}