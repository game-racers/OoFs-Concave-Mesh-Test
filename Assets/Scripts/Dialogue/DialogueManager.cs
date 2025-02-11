using gameracers.MiniGolf.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace gameracers.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] TextAsset introText;
        [SerializeField] TextAsset holeText;
        [SerializeField] TextAsset winText;

        DialogueBox db;

        public static DialogueManager dm;

        private void Start()
        {
            db = transform.GetChild(0).GetComponent<DialogueBox>();

            if (dm == null)
                dm = this;
        }

        public void NextHoleText(int val)
        {
            string[] line = holeText.text.Split("\n");
            db.UpdateText("Game Master", line[val]);
        }

        public void DisplayWinText(int val)
        {
            string[] line = winText.text.Split("\n");
            db.UpdateText("Game Master", line[val]);
        }

        public void CloseTextBauble()
        {
            db.gameObject.SetActive(false);
        }
    }
}
