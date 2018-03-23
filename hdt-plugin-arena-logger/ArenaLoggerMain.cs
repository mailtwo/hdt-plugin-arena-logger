using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Hearthstone;
using HearthWatcher.EventArgs;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Plugins;

namespace hdt_plugin_arena_logger
{
    public class ArenaLoggerMain : IPlugin
    {
        private bool _enabled = false;

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
                string choicesName = MakeChoicesString(args.Choices);
                Log.Info("Choice changed. Choices: [" + choicesName + "}]");
            }
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
                    Log.Info("Card picked. Pick: [" + pickName + "], Choices: [" + choicesName + "]");
            }

        }

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
            return curCard.Name + "\\" + curCard.Id;
        }

        public Version Version
        {
            get { return new Version(0, 1, 1); }
        }

        public MenuItem MenuItem => null;
    }
}
