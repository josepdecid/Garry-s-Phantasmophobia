using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateParams
{
    public float patrolSpeed { get; }
    public float hideTimeout { get; }
    public float searchTimeout { get; }
    
    public float fleeSpeed { get; }
    public int numSamples { get; }
    public float samplingRadius { get; }
    public float maxSamplingDistance { get; }

    public bool isDebug { get; }
    public Gradient candidateGradient { get; }

    public StateParams(
        float patrolSpeed, float hideTimeout, float searchTimeout,
        float fleeSpeed, int numSamples, float samplingRadius, float maxSamplingDistance,
        bool isDebug, Gradient candidateGradient)
    {
        this.patrolSpeed = patrolSpeed;
        this.hideTimeout = hideTimeout;
        this.searchTimeout = searchTimeout;

        this.fleeSpeed = fleeSpeed;
        this.numSamples = numSamples;
        this.samplingRadius = samplingRadius;
        this.maxSamplingDistance = maxSamplingDistance;

        this.isDebug = isDebug;
        this.candidateGradient = candidateGradient;
    }
}
