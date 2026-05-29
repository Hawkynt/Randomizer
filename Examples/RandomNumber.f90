! Modern Fortran (2008+): the intrinsic random_number returns reals in [0, 1).
! Use random_init / random_seed to seed reproducibly. The implementation is
! compiler-dependent (gfortran ships Mersenne Twister) and is NOT
! cryptographically secure — for crypto you need a C interop layer.
!
! Build: gfortran -o RandomNumber RandomNumber.f90

program random_64bit
  use iso_fortran_env, only: int64, real64
  implicit none

  real(real64) :: r1, r2
  integer(int64) :: n
  integer(int64), parameter :: scale32 = 4294967296_int64    ! 2^32
  integer(int64), parameter :: mask32  = 4294967295_int64    ! 2^32 - 1

  call random_init(repeatable=.false., image_distinct=.false.)
  call random_number(r1)
  call random_number(r2)

  ! Combine two doubles into a 64-bit integer (32 bits each).
  n = ior(ishft(int(r1 * real(scale32, real64), int64), 32), &
          iand(int(r2 * real(scale32, real64), int64), mask32))

  write(*, '("Random 64-bit number: ", z16.16)') n
end program random_64bit
