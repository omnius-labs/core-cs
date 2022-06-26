use clap::{App, Arg};
use rand::RngCore;
use rand_xoshiro::rand_core::SeedableRng;
use rand_xoshiro::Xoshiro256StarStar;
use sha2::{Digest, Sha256};
use std::process;
use std::sync::{Arc, Mutex};
use std::thread;
use std::time::{Duration, Instant};

fn simple_sha2_256(value: &[u8]) -> anyhow::Result<()> {
    let mut rng = Xoshiro256StarStar::from_rng(rand::thread_rng())?;

    let buffer: &mut [u8] = &mut [0; 64];
    buffer[32..].copy_from_slice(value);

    let mut last_difficulty: u32 = 0;

    loop {
        buffer[0..8].copy_from_slice(&rng.next_u64().to_ne_bytes());
        buffer[8..16].copy_from_slice(&rng.next_u64().to_ne_bytes());
        buffer[16..24].copy_from_slice(&rng.next_u64().to_ne_bytes());
        buffer[24..32].copy_from_slice(&rng.next_u64().to_ne_bytes());

        let mut hasher = Sha256::new();
        hasher.update(&buffer);
        let result = hasher.finalize();

        let mut current_difficulty: u32 = 0;

        for element in result {
            let zeros = element.leading_zeros();
            current_difficulty += zeros;

            if zeros < 8 {
                break;
            }
        }

        if last_difficulty < current_difficulty {
            println!("{} {}", current_difficulty, base64::encode(&buffer[0..32]));
            last_difficulty = current_difficulty;
        }
    }
}

fn main() -> anyhow::Result<()> {
    {
        let matches = App::new("Hashcash")
            .version("1.0")
            .arg(
                Arg::with_name("type")
                    .long("type")
                    .takes_value(true)
                    .required(true)
                    .help("type"),
            )
            .arg(
                Arg::with_name("value")
                    .long("value")
                    .takes_value(true)
                    .required(true)
                    .help("value"),
            )
            .get_matches();

        let hashcash_type = matches.value_of("type").unwrap();
        let value = base64::decode(matches.value_of("value").unwrap()).unwrap();

        if hashcash_type == "simple_sha2_256" {
            thread::spawn(move || simple_sha2_256(value.as_slice()));
        } else {
            println!("Incorrect type <{}>", hashcash_type);
            process::exit(1);
        }
    }

    {
        let time_to_receive_alive = Arc::new(Mutex::new(Instant::now()));

        {
            let time_to_receive_alive = time_to_receive_alive.clone();

            thread::spawn(move || loop {
                thread::sleep(Duration::from_secs(1));
                let result = time_to_receive_alive.lock().unwrap();

                if result.elapsed().as_secs() > 5 {
                    process::exit(1);
                }
            });
        }

        {
            let time_to_receive_alive = time_to_receive_alive.clone();

            thread::spawn(move || loop {
                let mut input = String::new();
                std::io::stdin().read_line(&mut input).unwrap();
                let input = input.trim();

                if input == "a" {
                    let mut time_to_receive_alive = time_to_receive_alive.lock().unwrap();
                    *time_to_receive_alive = Instant::now();
                } else if input == "e" {
                    process::exit(1);
                }
            })
            .join()
            .unwrap();
        }
    }

    Ok(())
}
