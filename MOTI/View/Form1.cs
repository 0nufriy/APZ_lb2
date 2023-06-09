using Backend.Core.Interfaces;
using Backend.Core.Models;
using Backend.Core.Services;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Models;
using MOTI.Model;
using MOTI.Model.Audience;
using MOTI.Model.AudiencePack;
using System.Reflection.Emit;

namespace MOTI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Backend.Infrastructure.Data.ApplicationContext _context;

        public IAudienceService _service;

        List<AudienceGetDTO> audienceGetDTOs;
        List<AudiencePackGetDTO> audiencePackGetDTOs;
        public List<string> audienceNeed = new List<string>();

        public int count;

        public async void LodingDataBase()
        {
            audienceGetDTOs = await _service.GetAudience();

            audiencePackGetDTOs = await _service.GetAudiencePack();

            AudienceList.Items.Clear();
            AudiencePackList.Items.Clear();

            foreach (var audience in audienceGetDTOs)
            {
                AudienceList.Items.Add(audience.AudienceId.ToString() + " " + audience.Age.ToString() + " " + (audience.Sex ? "Man" : "Woman"));
            }


            foreach (var pack in audiencePackGetDTOs)
            {
                AudiencePackList.Items.Add(pack.AudiencePackId.ToString() + " " + pack.AudiencePackName + " " + pack.Price.ToString());
            }
        }

        public void UpdateNeed()
        {
            AudienceNeed.Items.Clear();
            foreach (var ss in audienceNeed)
            {
                AudienceNeed.Items.Add(ss);
            }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            _context = new Backend.Infrastructure.Data.ApplicationContext(new Microsoft.EntityFrameworkCore.DbContextOptions<Backend.Infrastructure.Data.ApplicationContext>());
            _service = new AudienceService(_context);

            LodingDataBase();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (AudienceList.SelectedItems.Count == 0)
            {
                return;
            }
            int id = Convert.ToInt32(AudienceList.SelectedItems[0].ToString().Split(" ")[0]);
            var windows = new SelectAudience(id, this);
            windows.ShowDialog();
            if (count == 0)
            {
                return;
            }
            audienceNeed.Add(AudienceList.SelectedItems[0].ToString() + "   x" + count.ToString());
            UpdateNeed();
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var windows = new AddAudience(this);
            windows.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (AudienceList.SelectedItems.Count == 0)
            {
                return;
            }
            bool sex = AudienceList.SelectedItems[0].ToString().Split(" ")[2] == "Man";
            int age = Convert.ToInt32(AudienceList.SelectedItems[0].ToString().Split(" ")[1]);
            int id = Convert.ToInt32(AudienceList.SelectedItems[0].ToString().Split(" ")[0]);
            var windows = new EditAudience(this, age, sex, id);
            windows.ShowDialog();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (AudienceList.SelectedItems.Count == 0)
            {
                return;
            }
            var message = MessageBox.Show("ĳ���� ��������?", "�������?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (message == DialogResult.Yes)
            {
                await _service.DeleteAudience(Convert.ToInt32(AudienceList.SelectedItems[0].ToString().Split(" ")[0]));

                LodingDataBase();
            }
            return;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (AudienceNeed.SelectedItems.Count == 0)
            {
                return;
            }
            EditNeedAudience windows = new EditNeedAudience(this, AudienceNeed.SelectedItems[0].ToString(), AudienceNeed.SelectedIndex);
            windows.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {

            if (AudienceNeed.SelectedItems.Count == 0)
            {
                return;
            }
            var message = MessageBox.Show("ĳ���� ��������?", "�������?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (message == DialogResult.Yes)
            {
                audienceNeed.Remove(audienceNeed[AudienceNeed.SelectedIndex]);
            }
            UpdateNeed();
            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (AudiencePackList.SelectedItems.Count == 0)
            {
                return;
            }
            int id = Convert.ToInt32(AudiencePackList.SelectedItems[0].ToString().Split(" ")[0]);

            var window = new SelectAudiencePack(this, audiencePackGetDTOs[AudiencePackList.SelectedIndex], audienceGetDTOs, true);
            window.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var window = new AddAudiencePack(this, audienceGetDTOs);
            window.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (AudiencePackNeed.SelectedItems.Count == 0)
            {
                return;
            }
            int id = Convert.ToInt32(AudiencePackNeed.SelectedItems[0].ToString().Split(" ")[0]);


            var window = new SelectAudiencePack(this, audiencePackGetDTOs.FirstOrDefault(x => x.AudiencePackId == id), audienceGetDTOs, false);
            window.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(audienceNeed.Count == 0)
            {
                return;
            }
            List<AudienceNeed> listAu = new List<AudienceNeed>();
            foreach( var item in audienceNeed )
            {
                listAu.Add(new Model.AudienceNeed
                {
                    id = Convert.ToInt32(item.ToString().Split(" ")[0]),
                    age = Convert.ToInt32(item.ToString().Split(" ")[1]),
                    sex = item.ToString().Split(" ")[2],
                    count = Convert.ToInt32(item.ToString().Split("x")[1])
                });
            }
            List<AudiencePackNeed> listAp = new List<AudiencePackNeed>();
            foreach ( var item in audiencePackGetDTOs)
            {
                decimal priceByOnePerson = 0;
                foreach ( var item2 in item.Audiences)
                {
                    priceByOnePerson += item2.AudienceCount;
                }
                priceByOnePerson = item.Price / priceByOnePerson;
                listAp.Add(new Model.AudiencePackNeed()
                {
                    AudiencePackId = item.AudiencePackId,
                    AudiencePackName = item.AudiencePackName,
                    Price = item.Price,
                    Audiences = item.Audiences,
                    PriceByOnePerson = priceByOnePerson,
                });
            }

            listAp.Sort((a,b) => a.PriceByOnePerson.CompareTo(b.PriceByOnePerson));

            for(int i = 0; i < listAp.Count; i++)
            {
                if (listAp[i].Audiences.Select(x => x.AudienceId).All(x => listAu.Select(y => y.id).Contains(x))
                    && (listAp[i].Audiences.All(x => listAu.FirstOrDefault(y => y.id == x.AudienceId).count >= x.AudienceCount)))
                {
                    listAp[i].count++;
                    foreach(var a in listAp[i].Audiences)
                    {
                        listAu.FirstOrDefault(x => x.id == a.AudienceId).count -= a.AudienceCount;
                    }
                    i--;
                }
            }

            foreach ( var pack in listAp)
            {
                if(pack.count > 0)
                {

                    AudiencePackNeed.Items.Add(pack.AudiencePackId.ToString() + " " + pack.AudiencePackName + " " + pack.Price.ToString() + " x" + pack.count);
                }
            }

        }
    }
}