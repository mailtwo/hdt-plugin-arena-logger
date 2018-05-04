using System;
using System.Windows.Controls;
using System.Threading.Tasks;
using HearthMirror;
using Hearthstone_Deck_Tracker.Hearthstone;
using HearthWatcher.EventArgs;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Plugins;
using HearthWatcher.Providers;

namespace hdt_plugin_arena_logger
{
    public class ArenaLoggerMain : IPlugin
    {
        private HearthMirrorArenaProvider _provider = new HearthMirrorArenaProvider();
        private bool _enabled = false;
        private HearthMirror.Objects.Card[] _prevChoices = null;
        private HearthMirror.Objects.Card[] _prevChoices2 = null;

        public string Author
        {
            get { return "mailtwo"; }
        }
        public string Name
        {
            get { return "ArenaLogger"; }
        }

        public string ButtonText
        {
            get { return "Settings"; }
        }

        public string Description
        {
            get { return "Add log while picking cards in the arena."; }
        }
        public void OnButtonPress()
        {
        }
        public void OnUpdate()
        {
        }
        public void OnLoad()
        {
            _enabled = true;
            Watchers.ArenaWatcher.OnChoicesChanged += OnChoicesChanged;
            Watchers.ArenaWatcher.OnCardPicked += OnCardPicked;
        }

        public void OnUnload()
        {
            _enabled = false;
        }

        public void OnChoicesChanged(object sender, ChoicesChangedEventArgs args)
        {
            if (_enabled)
            {
                OnChoicesChanged_Internal(sender, args);
            }
        }

        private async void OnChoicesChanged_Internal(object sender, ChoicesChangedEventArgs args)
        {
            var choices = _provider.GetDraftChoices();
            int count = 0;
            if (_prevChoices2 != null)
            {
                while (!ChoicesChanged(_prevChoices2, choices))
                {
                    await Task.Delay(100);
                    choices = _provider.GetDraftChoices();
                    if (count > 100)
                        break;
                    count += 1;
                }
            }
            string choicesName = MakeChoicesString(choices);
            Log.Info("Choice changed. Choices: [" + choicesName + "}]");
            _prevChoices2 = choices;
        }
        
        public void OnCardPicked(object sender, CardPickedEventArgs args)
        {
            if (_enabled)
            {
                Card pickedCard = Database.GetCardFromId(args.Picked.Id);
                string pickName = MakeCardString(args.Picked);
                string choicesName = MakeChoicesString(args.Choices);
                if (pickedCard.Type == "Hero")
                    Log.Info("Hero picked. Pick: [" + pickName + "], Choices: [" + choicesName + "]");
                else
                {
                    OnCardPicked_Internal(sender, args);
                }
            }

        }

        private async void OnCardPicked_Internal(object sender, CardPickedEventArgs args)
        {
            if (_prevChoices == null)
                _prevChoices = args.Choices;

            string pickName = MakeCardString(args.Picked);
            string choicesName = MakeChoicesString(_prevChoices);
            var choices = _provider.GetDraftChoices();
            int count = 0;
            while (!ChoicesChanged(_prevChoices, choices))
            {
                await Task.Delay(100);
                choices = _provider.GetDraftChoices();
                if (count > 100)
                    break;
                count += 1;
            }
            string curChoicesName = MakeChoicesString(choices);
            Log.Info("Card picked. Pick: [" + pickName + "], Choices: [" + choicesName + "], curChoices: [" + curChoicesName + "]");
            _prevChoices = choices;
        }

        private bool ChoicesChanged(HearthMirror.Objects.Card[] _prevChoices, HearthMirror.Objects.Card[] choices) => choices[0].Id.CompareTo(_prevChoices[0].Id) != 0 || choices[1].Id.CompareTo(_prevChoices[1].Id) != 0 || choices[2].Id.CompareTo(_prevChoices[2].Id) != 0;

        private string MakeChoicesString(HearthMirror.Objects.Card[] cardList)
        {
            string choicesName = "";
            foreach (HearthMirror.Objects.Card card in cardList)
            {
                choicesName += MakeCardString(card) + "; ";
            }
            return choicesName;
        }

        private string MakeCardString(HearthMirror.Objects.Card card)
        {
            Card curCard = Database.GetCardFromId(card.Id);
            return curCard.Name + "/" + curCard.Id;
        }

        public Version Version
        {
            get { return new Version(0, 1, 1); }
        }

        public MenuItem MenuItem => null;
    }
}
