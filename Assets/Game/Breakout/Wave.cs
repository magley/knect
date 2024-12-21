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

        ConvertChildBallObjectsIntoOnHitDestroy();
	}

    private void ConvertChildBallObjectsIntoOnHitDestroy()
    {
        // By doing this, we limit the upper bound of balls in the level to K + N
        // where K is the starting number of balls and N is the total amount of
        // extra balls in this wave. If we didn't do this, the upper bound would
        // be K + i * N where i the i-th wave currently being played.
        //
        // For this to work, all balls created from the current wave must actually
        // be children of the wave object.

        foreach (var ball in GetComponentsInChildren<Ball>())
        {
            ball.DestroyOnImpact = true;
		}
    }

    private void CallForNextWave()
    {
		levelManager = FindObjectOfType<LevelManager>();        // ???
		levelManager.SpawnNextWave();
    }
}
