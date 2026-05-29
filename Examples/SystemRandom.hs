-- Haskell: System.Random (in base) provides a fast non-cryptographic PRNG
-- (SplitMix). For crypto use the `crypton` (formerly `cryptonite`) package's
-- Crypto.Random.SystemDRG, which reads from /dev/urandom or the OS DRBG.
--
-- This example uses only `base`; the commented crypto block requires:
--   cabal install --lib crypton

import Data.Bits (shiftL, (.|.))
import Numeric (showHex)
import System.Random (randomIO)

main :: IO ()
main = do
  hi <- randomIO :: IO Int
  lo <- randomIO :: IO Int
  let n :: Integer
      n = (fromIntegral hi `shiftL` 32) .|. (fromIntegral lo .&. 0xFFFFFFFF)
  putStrLn $ "Random 64-bit number: " ++ pad16 (showHex (n `mod` (1 `shiftL` 64)) "")
  where
    pad16 s = replicate (16 - length s) '0' ++ s
