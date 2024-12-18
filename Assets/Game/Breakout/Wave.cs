using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

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
    private LevelManager levelManager;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        levelManager = FindObjectOfType<LevelManager>();
		particles.Emit(40);
		mode = Mode.Active;
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

    private void CallForNextWave()
    {
        levelManager.SpawnNextWave();
    }
}
