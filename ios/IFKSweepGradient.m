#import "IFKSweepGradient.h"

@implementation IFKSweepGradient
  
+ (void)initialize
{
  [self initializeWithGradientClass:[IFKSweepGradient class]
                        displayName:@"Sweep Gradient (max 5 colors)"];
}
  
+ (CIKernel *)filterKernel:(int)colorsAmount {
  static NSArray<CIKernel *> *kernels;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    kernels = [self loadKernels:[IFKSweepGradient class]];
  });
  
  return kernels[colorsAmount - 1];
}
  
- (CIVector *)inputCenter
{
  if (_inputCenter) {
    return _inputCenter;
  }
  
  return [CIVector vectorWithCGPoint:self.inputExtent
          ? CGPointMake(self.inputExtent.X + self.inputExtent.Z * 0.5,
                        self.inputExtent.Y + self.inputExtent.W * 0.5)
                                    : CGPointZero];
}
  
- (CIImage *)outputImage
{
  if (self.inputExtent == nil) {
    return nil;
  }
  
  int inputAmount = [self inputAmount];
  
  [IFKGradient assertMaxColors:[IFKSweepGradient class] inputAmount:inputAmount];
  
  CIKernel *kernel = [IFKSweepGradient filterKernel:inputAmount];
  NSMutableArray *args = [NSMutableArray array];
  
  for (int i = 0; i < inputAmount; i++) {
    [args addObject:self.inputColors[i]];
    [args addObject:@([self.inputStops valueAtIndex:i])];
  }
  
  [args addObject:self.inputCenter];
  
  NSLog(@"IFK: %@", args);
  
  return [kernel applyWithExtent:[self.inputExtent CGRectValue]
                     roiCallback:^CGRect(int index, CGRect destRect) {
                       return destRect;
                     } arguments:args];
}
@end
