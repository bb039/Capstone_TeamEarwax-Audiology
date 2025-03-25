using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public static string name;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void setName(string n) {
		name = n;
	}

	public string getName() {
		return name;
	}
}
