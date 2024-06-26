using FootballBotAPI.Clients;
using FootballBotAPI.Models.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace FootballBotAPI.Controllers
{

    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;
        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }
        [HttpGet("/APIController/GetFixturesAll")]
        public async Task<string> GetFixtures()
        {
            try
            {
                APIClient client = new APIClient();
                return await ParseResponse(await client.GetFixtures(), false, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������� ��� ������������ �ϲ");
                return "����� ������� �������. �������� ������� ����";
            }
        }

        [HttpGet("/APIController/GetFixturesByDate")]
        public async Task<string> GetFixtures(string date, bool IsToday)
        {
            try
            {
                APIClient client = new APIClient();
                return await ParseResponse(await client.GetFixtures(date), IsToday, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������� ��� ������������ �ϲ");
                return "����� ������� �������. �������� ������� ����";
            }
        }

        [HttpGet("/APIController/GetFixturesByTeamInSeason")]
        public async Task<string> GetFixtures(string teamName, ushort season)
        {
            try
            {
                APIClient client = new APIClient();
                return await ParseResponse(await client.GetFixtures((await client.GetTeam(teamName)).Response[0].Team.ID, season: season), false, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������� ��� ������������ �ϲ");
                return "����� ������� �������. �������� ������� ����";
            }
        }



        private async Task<string> ParseResponse(Fixtures content, bool IsToday, bool IsLive)
        {
            int counter = content.Response.Length > 15 ? 15 : content.Response.Length;
            if (counter == 0) return "�� ����, ���� ���������� �� ������ ������ �� ����� ������.";
            return await FormatAnswer(content, IsToday, IsLive, counter);
        }
        
        private async Task<string> FormatAnswer(Fixtures content, bool IsToday, bool IsLive, int counter)
        {
            string answer = "";
            for (int i = 0; i < counter; i++)
            {
                if (!IsLive)
                {
                    if (IsToday && content.Response[i].Fixture.Status.Long != "Not Started")
                    {
                        continue;
                    }

                    if (!IsToday && content.Response[i].Fixture.Status.Long != "Match Finished")
                    {
                        continue;
                    }
                }


                answer += content.Response[i].Teams.Home.Name + "-----" +
                    content.Response[i].Teams.Away.Name + "\n" +
                    content.Response[i].Goals.Home + "\t" + " : " + "\t" +
                    content.Response[i].Goals.Away + "\n" +
                    content.Response[i].Fixture.Status.Long + "\n" + "----------------------------" + "\n";
            }

            return answer;
        }
    }
}
