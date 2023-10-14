using UnityEngine;
using UnityEngine.UI;

public class UiEnemyHealthBar : MonoBehaviour
{
     Slider slider;
    float timeBarhidden=3.0f;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }
    public void SetHealth(int HP)
    {
        slider.value = HP;
        timeBarhidden = 3.0f;
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    private void Update()
    {
        timeBarhidden -= Time.deltaTime;
        if (slider != null)
        {

            if (timeBarhidden <= 0)
            {
                timeBarhidden = 0;
                slider.gameObject.SetActive(false);

            }

            else
            {
                if (!slider.gameObject.activeInHierarchy)
                {
                    slider.gameObject.SetActive(true);
                }
            }

            if (slider.value <= 0)

            {

                Destroy(slider.gameObject);

            }
        }

    }

}
