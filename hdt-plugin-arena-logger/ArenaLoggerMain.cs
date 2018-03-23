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
            Watchers.ArenaWatcher.OnCardPicked += OnCardPicked;
        }

        public void OnUnload()
        {
            _enabled = false;
        }
        
        public void OnCardPicked(object sender, CardPickedEventArgs args)
        {
            if (_enabled)
            {
                string pickName = Database.GetCardFromId(args.Picked.Id).Name;
                string choicesName = "";
                foreach (HearthMirror.Objects.Card card in args.Choices)
                {
                    Card curCard = Database.GetCardFromId(card.Id);
                    choicesName += curCard.Name + "; ";
                }
                Log.Info("Card picked. Pick: [" + pickName + "], Choices: [" + choicesName + "}]");
            }

        }
        public Version Version
        {
            get { return new Version(0, 1, 1); }
        }

        public MenuItem MenuItem => null;
    }
}
