using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    bool logoVisible = false;
    [SerializeField] private Image Logo;

    void Start()
    {
        Invoke(nameof(ShowLogo), 1);
    }

	private void Update()
	{
		if (logoVisible)
        {
            var c = Logo.color;
            c.a += 0.1f;
            c.a = Mathf.Min(c.a, 1);
            Logo.color = c;
		} 
        else
        {
			var c = Logo.color;
			c.a -= 0.075f;
			c.a = Mathf.Max(c.a, 0);
			Logo.color = c;
		}
	}

	private void ShowLogo()
    {
        GetComponent<AudioSource>().Play();
        logoVisible = true;
		Invoke(nameof(HideLogo), 2);
	}

    private void HideLogo()
    {
        logoVisible = false;
		Invoke(nameof(GoToMainMenu), 1);
	}

	private void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
