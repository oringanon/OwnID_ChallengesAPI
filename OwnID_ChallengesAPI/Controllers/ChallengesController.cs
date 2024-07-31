using Microsoft.AspNetCore.Mvc;
using OwnID_ChallengesAPI.ChallengesServices;
using OwnID_ChallengesAPI.Models.Requests;

namespace OwnID_ChallengesAPI.Controllers
{
    [ApiController]
    [Route("challenges/{appId}")]
    public class ChallengesController : ControllerBase
    {
        readonly ChallengesService _challengesService;
        public ChallengesController(ChallengesService challengesService)
        {
            _challengesService = challengesService;
        }
        [HttpPost("start")]
        public IActionResult StartChallenge([FromRoute] string appId, [FromBody] ChallengeRequest request)
        {
            try
            {
                var response = _challengesService.StartChallenge(appId, request);

                return Ok(new { response.ChallengeId });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("solve/{challengeId}")]
        public IActionResult Solve([FromRoute] string appId, [FromRoute] string challengeId, [FromBody] SolveRequest request)
        {
            try
            {
                var result = _challengesService.Solve(appId, challengeId, request.Otp);
                return Ok(new { success = result });

                //i wanted to use fido as a second check after the otp check in order to optimize my development on the 
                //security side - investigate the right strategy took some time and we agreed on 2 hours so adding comment 
                //this comment as if i had the time this is where i think this part should be :) 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
