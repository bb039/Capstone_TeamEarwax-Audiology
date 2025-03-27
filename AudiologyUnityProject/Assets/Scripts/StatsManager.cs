using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public static string name;

	public float elapsedTime;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void setName(string n) {
		name = n;
	}

	public string getName() {
		return name;
	}

    public void setElapsedTime(float time)
    {
        elapsedTime = time;
    }

    public float getElapsedTime()
    {
        return elapsedTime;
    }


}
