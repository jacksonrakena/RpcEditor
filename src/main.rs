use cursive::{
    view::{Finder, Margins, Nameable, Resizable},
    views::{Button, Dialog, EditView, LinearLayout, PaddedView, TextView},
    Cursive,
};
use discord_rpc_client::{models::Event, Client};

fn main() {
    let mut app = cursive::default();

    app.add_global_callback('q', |s| s.quit());

    let input = LinearLayout::vertical()
        .child(TextView::new("App ID"))
        .child(EditView::new().with_name("App ID").fixed_width(20));

    app.add_layer(
        Dialog::new()
            .content(input)
            .button("Connect", connect_start),
    );
    app.run();
}

fn connect_start(c: &mut Cursive) {
    let app_id = c
        .call_on_name("App ID", |v: &mut EditView| v.get_content())
        .unwrap();

    match app_id.parse::<u64>() {
        Ok(app_id_u64) => {
            let mut rpc_client = Client::new(app_id_u64);
            c.pop_layer();
            let state_view = TextView::new("Connecting...").with_name("state_text");
            c.add_fullscreen_layer(state_view);

            rpc_client.start();

            rpc_client
                .set_activity(|act| act.state("Rust"))
                .expect("failed to set activity");
            c.call_on_name("state_text", |v: &mut TextView| {
                v.set_content("Connected.");
            });

            let game_name_input = LinearLayout::horizontal()
                .child(TextView::new("Game name:"))
                .child(EditView::new().with_name("game_name").fixed_width(20));

            let game_state_input = LinearLayout::horizontal()
                .child(TextView::new("State:"))
                .child(EditView::new().with_name("game_state").fixed_width(20));

            let input_panel = PaddedView::new(
                Margins::lrtb(1, 1, 1, 1),
                LinearLayout::vertical()
                    .child(game_name_input)
                    .child(game_state_input),
            );

            c.pop_layer();
            c.add_fullscreen_layer(input_panel);
        }
        Err(_) => {
            return;
        }
    }
}
