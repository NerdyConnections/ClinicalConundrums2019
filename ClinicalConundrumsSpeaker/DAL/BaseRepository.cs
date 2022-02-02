using ClinicalConundrum2019.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.DAL
{
    public class BaseRepository
    {
        private ClinicalConundrums2019Entities ent = null;

        public ClinicalConundrums2019Entities Entities
        {
            get
            {
                if (ent == null)
                {
                    ent = new ClinicalConundrums2019Entities();
                }
                return ent;

            }
        }
    }
}