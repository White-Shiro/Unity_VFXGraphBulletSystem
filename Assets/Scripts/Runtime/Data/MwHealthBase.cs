using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwHealthBase : MwCharacterComponent
{
    //MwCharacter < MwGameObject < GameObject < Object < C# object

    [field:SerializeField] public float maxHp { get; protected set; } = 100f;
    [field:SerializeField] public float currentHP { get; protected set; } = 0;
    
    [SerializeField]            protected bool _damageable = true;  
    [SerializeField]            protected bool _healAble = true;

    [SerializeField] protected float _hurtInterval = 0.1f;
    [SerializeField] protected float _healInterval = 5f;

    [SerializeField][WxShowOnly]protected float _hurtTimer = 0f;
    [SerializeField][WxShowOnly]protected float _healTimer = 0f;

    public event Action onDamageAction, onDeadAction, onHealAction;

    //For Child Class Override
    protected override void OnConstruct(MwCharacter character_)
    {
        currentHP = maxHp;
    }

    private void OnEnable()
    {
        UnRegisterAllEvents();
        //onDeadAction +=
    }

    private void OnDisable()
    {
        UnRegisterAllEvents();
    }

    protected void Update()
    {
        // onDamageAction?.Invoke();
        //
        // if(_hp == 0)
        //     onDeadAction?.Invoke();
    }

    protected virtual void OnTakeDamage(MwDamage damage_)
    {
        ChangeHP(-damage_.damage);
        var dType = damage_.damageType;

        switch (dType)
        {
            case MwDamageType.Physical : Debug.Log("Physical"); break;
            case MwDamageType.Explosive: Debug.Log("Explosive");  break;
        }
    }

    protected virtual void OnTakeDamage(float damage_ = 0f)
    {
        ChangeHP(-damage_);
    }

    public void TakeDamage(MwDamage damage_, Action cb = null)
    {
        if (!_damageable) return;
        if (Time.time < _hurtTimer) return;
    
        //GameTime > hurtTimer do sth
        OnTakeDamage(damage_);
        cb?.Invoke();
    }

    public void TakeDamage(float damage_ = 0f, Action cb = null)
    {
        if (!_damageable) return;
        if (Time.time < _hurtTimer) return;
        
        ChangeHP(-damage_);
        cb?.Invoke();
    }
    
    protected virtual void OnHeal(float healingAmount_)
    {
        ChangeHP(healingAmount_);
    }

    public void Heal(float healingAmount_, Action cb = null)
    {
        if (!_healAble) return;
        
        if(Time.time < _healTimer) return;

        OnHeal(healingAmount_);
        cb?.Invoke();
    }

    protected virtual void OnDie()
    {
        Debug.Log($"{transform.name} is dead");
    }

    protected virtual void ChangeHP(float value)
    {
        currentHP += value;
        //OnHPChange?.invoke();

        if (value < 0)
        {
            _hurtTimer = Time.time + _hurtInterval;
        }
        else if (value > 0)
        {
            _healTimer = Time.time + _healInterval;
        }

        else
        {
            _hurtTimer = Time.time + _hurtInterval;
            _healTimer = Time.time + _healInterval;
        }

        if (currentHP > 0) return;
        OnDie();
    }

    public void RegisterDeadEvent(Action eventCallback)
    {
        onDeadAction -= eventCallback;
        onDeadAction += eventCallback;
    }

    public void UnRegisterDeadEvent(Action eventCallback)
    {
        onDeadAction -= eventCallback;
    }
    public virtual void UnRegisterAllEvents()
    {
        onDamageAction = null;
        onDeadAction = null;
        onHealAction = null;
    }

    [ContextMenu("TestTakeDmg")]

    public void TestDmg()
    {
        TakeDamage(10f);
    }
    
    [ContextMenu("TestHeal")]

    public void TestHeal()
    {
        Heal(3f);
    }
}
