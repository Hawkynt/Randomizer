use std::arch::x86_64::_rdseed64_step;

fn main() {
    let mut random_number: u64 = 0;
    let success: i32;

    unsafe {
        success = _rdseed64_step(&mut random_number);
    }

    if success == 1 {
        println!("Random 64-bit number: {:016x}", random_number);
    } else {
        println!("Failed to generate random number");
    }
}