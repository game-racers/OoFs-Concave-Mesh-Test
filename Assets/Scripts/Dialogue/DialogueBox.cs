using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace gameracers.Dialogue
{
    public class DialogueBox : MonoBehaviour
    {
        TextMeshProUGUI mainText;
        TextMeshProUGUI nameText;
        

        void Awake()
        {
            mainText = transform.Find("Text Box").Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            nameText = transform.Find("Text Box").Find("Name Plate").Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        }

        public void UpdateText(string name, string dialogue)
        {
            mainText.text = dialogue;
            nameText.text = name;
            gameObject.SetActive(true);
        }
    }
}
