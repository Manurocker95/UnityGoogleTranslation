using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace ManuelRodriguezMatesanz
{
    public class GoogleTranslation : MonoBehaviour
    {
        [Header("Text Data")]
        public string m_textToTranslate = "Hello world";
        public SystemLanguage m_languageFrom = SystemLanguage.English;
        public SystemLanguage m_languageTo = SystemLanguage.Spanish;

        [Header("Unity UI")]
        public Text m_nonTrans;
        public Text m_trans;
        public Dropdown m_language1DD;
        public Dropdown m_language2DD;
        public Button m_translateTextButton;

        // to add more languages: https://ctrlq.org/code/19899-google-translate-languages
        public Dictionary<SystemLanguage, string> m_availableLanguages = new Dictionary<SystemLanguage, string>()
        {
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.English, "en" },
            { SystemLanguage.French, "fr" },
            { SystemLanguage.German, "de" }

        };

        // Start is called before the first frame update
        void Start()
        {

            Init();
            ClickedTranslate();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClickedTranslate();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void Init()
        {
            m_nonTrans.text = m_textToTranslate;

            m_language1DD.ClearOptions();
            m_language2DD.ClearOptions();
            List<Dropdown.OptionData> languages = new List<Dropdown.OptionData>();
            int v1 = 0;
            int v2 = 0;
            int counter = 0;
            foreach (SystemLanguage language in m_availableLanguages.Keys)
            {
                languages.Add(new Dropdown.OptionData(language.ToString()));
                if (language == m_languageFrom)
                {
                    v1 = counter;
                }
                if (language == m_languageTo)
                {
                    v2 = counter;
                }

                counter++;
            }

            m_language1DD.AddOptions(languages);
            m_language2DD.AddOptions(languages);

            m_language1DD.value = v1;
            m_language2DD.value = v2;
        }

        private void OnEnable()
        {
            StartListeners();
        }

        private void OnDisable()
        {
            StopListeners();
        }

        public void StartListeners()
        {
            m_language1DD?.onValueChanged.AddListener(delegate { OnLanguageChange(); });
            m_language2DD?.onValueChanged.AddListener(delegate { OnLanguageChange(); });
            m_translateTextButton?.onClick.AddListener(delegate { ClickedTranslate(); });
        }

        public void StopListeners()
        {
            m_language1DD?.onValueChanged.RemoveListener(delegate { OnLanguageChange(); });
            m_language2DD?.onValueChanged.RemoveListener(delegate { OnLanguageChange(); });
            m_translateTextButton?.onClick.RemoveListener(delegate { ClickedTranslate(); });
        }

        public void OnLanguageChange()
        {
            System.Enum.TryParse(m_language1DD.captionText.text, out m_languageFrom);
            System.Enum.TryParse(m_language2DD.captionText.text, out m_languageTo);
        }

        public void ClickedTranslate()
        {
            if (!m_availableLanguages.ContainsKey(m_languageFrom))
            {
                m_languageFrom = SystemLanguage.English;
            }

            if (!m_availableLanguages.ContainsKey(m_languageTo))
            {
                m_languageTo = SystemLanguage.Spanish;
            }

            string lang1Key = m_availableLanguages[m_languageFrom];
            string lang2Key = m_availableLanguages[m_languageTo];

            m_trans.text = "Translated: " + TranslateText(m_nonTrans.text, lang1Key, lang2Key);
        }

        public string TranslateText(string text, string language1, string language2)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={language1}&tl={language2}&dt=t&q={Uri.EscapeUriString(text)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {
                return "Error Google Translation not found";
            }
        }
    }
}

