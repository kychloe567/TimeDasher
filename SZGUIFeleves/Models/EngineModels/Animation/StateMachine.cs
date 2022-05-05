﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class StateMachine
    {
        [JsonProperty]
        private Dictionary<string, Animation> States { get; set; }
        [JsonProperty]
        public string CurrentState { get; set; }

        [JsonIgnore]
        public BitmapImage CurrentTexture
        {
            get
            {
                return States[CurrentState].CurrentTexture;
            }
        }

        public StateMachine()
        {
            States = new Dictionary<string, Animation>();
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

        public void LoadTextures()
        {
            foreach (KeyValuePair<string, Animation> pair in States)
                pair.Value.LoadTextures();
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

        public static StateMachine GetPlayerDefault()
        {
            string objectsPath = "Textures\\Character";

            StateMachine sm = new StateMachine();

            Animation idle = new Animation("idle");
            foreach (var image in new DirectoryInfo(objectsPath + "\\idle").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                idle.AddTexture(bi, image.FullName, 0.2);
            }
            sm.AddState(idle);

            Animation runright = new Animation("runright");
            foreach (var image in new DirectoryInfo(objectsPath + "\\runright").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                runright.AddTexture(bi, image.FullName, 0.15);
            }
            sm.AddState(runright);

            Animation runleft = new Animation("runleft");
            foreach (var image in new DirectoryInfo(objectsPath + "\\runleft").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                runleft.AddTexture(bi, image.FullName, 0.15);
            }
            sm.AddState(runleft);

            sm.SetState("idle");
            return sm;
        }
    }
}
