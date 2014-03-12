using UnityEngine;
using System.Collections;

public class TransformationController : MonoBehaviour {

	public GameObject shipModel;
	public GameObject transformationAnimation;
	public GameObject characterModel;

	private enum State {
		None,
		Ship,
		Transforming,
		Robot,
		Sentinel
	};

	private string modelName = "Model";

	private State state = State.Ship;
	private State goalState = State.None;
	
		// Use this for initialization
	void Start () {
		// Which prefab should we have in the next state?
		GameObject prefab = GetPrefabForState(state);

		// Use the prefab as the "model"
		SetModelPrefab(prefab);
	}

	bool CanToggleState()
	{
		if(state == State.Transforming)
			return false;
		return true;
	}

	// Given the current state, what is the next state we should be in?
	State GetNextState()
	{
		// If we've already reached the goal state, do nothing
		if(goalState == state)
			return state;

		// Otherwise, we're going to try to move towards the goal state,
		// which requires us to into the transforming state first
		State nextState = State.None;
		if(state == State.Transforming)
			nextState = goalState;
		else
			nextState = State.Transforming;
			
		return nextState;
	}

	GameObject GetPrefabForState(State state)
	{
		switch(state)
		{
			case State.Ship:
				return shipModel;

			case State.Transforming:
				return transformationAnimation;

			case State.Robot:
				return characterModel;

			default:
				DebugUtils.Assert(false);
				return null;
		}
	}

	// Instanciates and connects the prefab, then returns it
	GameObject SetModelPrefab(GameObject prefab)
	{
		// Get rid of any existing model prefabs
		Transform child = transform.Find(modelName);
		if(child)
		{
			DestroyObject(child.gameObject);
		}

		// Create a new instance from the given prefab and attach it as a child
		GameObject instance = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), prefab.transform.rotation);
		instance.name = modelName;
		instance.transform.parent = transform;

		return instance;
	}

	// 
	string GetTransformAnimationName()
	{
		if(goalState == State.Robot)
			return "ToRobot";
		else
			return "ToShip";
	}
		
	void GotoNextState()
	{
		// What's the next state we should be in?
		State nextState = GetNextState();

		// Which prefab should we have in the next state?
		GameObject prefab = GetPrefabForState(nextState);

		// Use the prefab as the "model"
		GameObject child = SetModelPrefab(prefab);

		state = nextState;

		if(state == State.Transforming)
		{
			string animationName = GetTransformAnimationName();
			StartCoroutine(PlayTransformation(child.animation, animationName));
		}
	}

	public IEnumerator PlayTransformation(Animation animation, string animationName)
	{
		animation.Play(animationName);
		while(animation.IsPlaying(animationName))
			yield return null;
		GotoNextState();
	}

	// Update is called once per frame
	void Update () {
		bool stateChangeRequest = false;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(CanToggleState())
			{
				if(state == State.Ship)
					goalState = State.Robot;
				else
					goalState = State.Ship;

				stateChangeRequest = true;
			}
		}

		if(stateChangeRequest)
		{
			GotoNextState();
		}
	}
}
