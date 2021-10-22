using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReSys
{
    class UserMotionSubject : Subject
    {
        private int useId;
        private int movieId;
        private int submitScore;

        public UserMotionSubject(int _userId)
        {
            this.useId = _userId;
        }

        public override void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public override void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public override void Notify(string messageType)
        {
            foreach (object obs in observers)
            {
                ((Observer)obs).Update(messageType, useId, movieId, submitScore);
            }
        }

        public void SubmitScore(int _movieId, int _submitScore)
        {
            this.movieId = _movieId;
            this.submitScore = _submitScore;
            Notify("Score");
        }
    }
}
