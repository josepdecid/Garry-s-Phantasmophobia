using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State currentState;
    private FleeState flee;
    private PatrolState patrol;
    private HideState hide;
    public Animator animator;
    AnimatorClipInfo[] m_CurrentClipInfo;
    string m_ClipName;

    void Start()
    {
        this.Initialize(new PatrolState(animator));
    }

    void Update()
    {
        m_CurrentClipInfo= animator.GetCurrentAnimatorClipInfo(0);
   
        if(m_ClipName != m_CurrentClipInfo[0].clip.name) {
            m_ClipName = m_CurrentClipInfo[0].clip.name;
            Debug.Log(m_ClipName);
            switch (m_ClipName)
            {
                case "Patrol":
                    this.currentState = new PatrolState(animator); 
                    break;
                case "Flee":
                    this.currentState = new FleeState(animator); 
                    break;
                case "Hide":
                    this.currentState = new HideState(animator);
                    break;
                default:
                    break;
            }
            
        }

        this.currentState.StateUpdate();

    }


    public void Initialize(State startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
