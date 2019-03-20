using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using SeymourBot.Attributes;
using System.Linq;
using SeymourBot.DataAccess.StorageManager;

namespace SeymourBot.Modules
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        ImageFormat png = ImageFormat.Png;
        [Command("info")]
        [DevOrAdmin]
        private async Task GetInfo(IGuildUser user = null)
        {
            try
            {
                if (user == null)
                {
                    user = Context.Message.Author as IGuildUser;
                }

                var userSocketGuild = user as SocketGuildUser;
                string userAvatarURL = user.GetAvatarUrl(png, 1024);
                string userStatus = user.Status.ToString();
                var userCreatedAtString = user.CreatedAt.ToString("yyyy/MM/dd hh:mm");
                var userJoinedAtString = user.JoinedAt.Value.ToString("yyyy/MM/dd hh:mm");
                var userDiscriminator = user.Discriminator;
                string userActivity = user.Activity == null ? "nothing" : user.Activity.Name;


                var embed = new EmbedBuilder();
                embed.WithTitle($"{user.Username}{userDiscriminator}");
                embed.AddField("Account Created: ", userCreatedAtString, true);
                embed.AddField("Joined Melee Slasher: ", userJoinedAtString, true);
                embed.AddField("User ID: ", user.Id, false);
                embed.AddField("Currently playing ", userActivity, true);
                embed.AddField("Status: ", userStatus, true);
                embed.WithThumbnailUrl(userAvatarURL);
                embed.WithColor(new Color(0, 204, 255));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("Failed to display embed");
            }

        }

        [Command("forums")]
        private async Task InfoForums()
        {
            await Context.Channel.SendMessageAsync("https://mordhau.com/forum/");
        }


        [Command("avatar")]
        private async Task PullAvatar(IGuildUser user)
        {
            string avatarURL = user.GetAvatarUrl(ImageFormat.Auto, 1024);

            if (avatarURL is null)
            {
                await Context.Message.Channel.SendMessageAsync($"{user.Mention} does not have a pfp");
                return;
            }
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(0, 204, 255));
            embed.WithTitle($"{user.Username}'s avatar");
            embed.WithUrl(avatarURL);
            embed.WithImageUrl(avatarURL);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }









        [Command("NewCommand")]
        private async Task StoreInfoCommand([Remainder]string UserInput)
        {
            try
            {

                string[] splitInput = UserInput.Split(' ');
                string commandName = splitInput[0];
                string commandContent = String.Join(' ', splitInput.Skip(0));

                InformationStorageManager infoManager = new InformationStorageManager();

                await infoManager.StoreInfoCommandAsync(commandName, commandContent);

            }
            catch (Exception)
            {

                throw;
            }

        }

        [Command("c")]
        [Alias("i", "info")]
        private async Task PostInfoCommand(string cmdName)
        {
            try
            {
                InformationStorageManager infoManager = new InformationStorageManager();


                string commandContent = await infoManager.GetInfoCommand(cmdName);
                await Context.Channel.SendMessageAsync(commandContent);

            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync("u fucked up bitch " + ex.Message);
            }
        }
    }
}
