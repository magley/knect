using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine.Assertions;
using static Unity.VisualScripting.Metadata;

public class Wave : MonoBehaviour
{
    private enum Mode
    {
        Pending,
        Active,
        Completed,
    }
    private Mode mode = Mode.Pending;
    private ParticleSystem particles;
    private List<Transform> children = new();

    /// <summary>
    /// Reference to all waves in the level.
    /// </summary>
    private Wave[] waves = { };

    void Start()
    {
        waves = transform.parent.GetComponentsInChildren<Wave>();
        particles = GetComponent<ParticleSystem>();
		foreach (var child in GetComponentsInChildren<Transform>().Skip(1)) // Skip self.
        {
            children.Add(child);
		}

		SetActiveStateToAllChildren(false);
		InitializeFirstWaveIfNoneAreReady();
	}

    private void InitializeFirstWaveIfNoneAreReady()
    {
        if (waves[0].mode == Mode.Pending && waves[0] == this)
        {
            waves[0].ActivateWave();
        }
    }

    private void SetActiveStateToAllChildren(bool active)
    {
		foreach (var child in children)
		{
			child.gameObject.SetActive(active);
		}
	}

    void Update()
    {
        switch (mode)
        {
            case Mode.Pending:
                break;
            case Mode.Active:
				if (IsWaveCompleted())
				{
					CompleteWave();
                    Assert.IsTrue(mode == Mode.Completed);
				}
                break;
            case Mode.Completed:
                break;

		}
	}

    private bool IsWaveCompleted()
    {
        return gameObject.GetComponentsInChildren<Brick>().Count() == 0;
    }

    private void CompleteWave()
    {
		mode = Mode.Completed;
        Invoke(nameof(CallForNextWave), 3);
    }

    public void ActivateWave()
    {
        particles.Emit(40);
        mode = Mode.Active;
        SetActiveStateToAllChildren(true);
	}

    private void CallForNextWave()
    {
        foreach (var wave in waves)
        {
            if (wave.mode == Mode.Pending)
            {
                wave.ActivateWave();
                return;
			}
		}

        Debug.Log("Level completed!");
    }
}
