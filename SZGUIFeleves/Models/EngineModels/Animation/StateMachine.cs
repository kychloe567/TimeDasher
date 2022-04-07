using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public class StateMachine
    {
        private Dictionary<string, Animation> States { get; set; }
        private string CurrentState { get; set; }
        public BitmapImage CurrentTexture
        {
            get
            {
                return States[CurrentState].CurrentTexture;
            }
        }

        public StateMachine(Dictionary<string, Animation> states)
        {
            States = states;
            CurrentState = states.First().Value.Title;
        }
        public StateMachine(Animation state)
        {
            States = new Dictionary<string, Animation>();
            States.Add(state.Title, state);
            CurrentState = state.Title;
        }

        public void AddState(Animation an)
        {
            States.Add(an.Title, an);
        }

        public void SetState(string state)
        {
            CurrentState = state;
            if(States.ContainsKey(CurrentState))
                States[CurrentState].StartAnimation();
        }

        public void Update()
        {
            if (States.ContainsKey(CurrentState))
                States[CurrentState].UpdateAnimation();
        }

        public StateMachine GetCopy()
        {
            Dictionary<string, Animation> states = new Dictionary<string, Animation>();
            foreach(KeyValuePair<string, Animation> pair in States)
                states.Add(pair.Key, pair.Value.GetCopy());
            return new StateMachine(states)
            {
                CurrentState = CurrentState
            };
        }
    }
}
