import cv2
import numpy as np

start_point = (1538, 3257)
pivot = (256, 889)
#angle = 124.3907145
angle = 126.86989764584402129685561255909
halfAngle = angle/2
size = (512, 1024)
x_iter_left = 6
x_iter_right = 6
y_iter = 2
is_cutting_front = True

rotation_left_matrix = np.array([[np.cos(halfAngle), -np.sin(halfAngle)], [np.sin(halfAngle), np.cos(halfAngle)]])
rotation_right_matrix = np.array([[np.cos(-halfAngle), -np.sin(-halfAngle)], [np.sin(-halfAngle), np.cos(-halfAngle)]])

def get_standard(position, cur_pivot, cur_size):
    step = float(cur_size[1] - cur_pivot[1])
    
    down_point = np.array([position[0], position[1]])
    up_point = np.array([position[0], position[1] - cur_size[1]])
    
    down_pivot_point = np.array([down_point[0], down_point[1] - step])
    up_pivot_point = np.array([up_point[0], up_point[1] + step])
    
    down_pivot_double_point = np.array([down_point[0], down_point[1] - step * 2.0])
    up_pivot_double_point = np.array([up_point[0], up_point[1] + step * 2.0])
    
    return (down_pivot_double_point, up_pivot_double_point, down_pivot_point, up_pivot_point, down_point, up_point)

def get_each_vector(cur_pivot, cur_size):
    height = float(cur_size[1] - cur_pivot[1])
    tan_value = np.tan(np.deg2rad(halfAngle)) * height
    print
    
    
    uleft_vector = np.array([-tan_value, -height])
    uright_vector =np.array([tan_value, -height])
    
    dleft_vector = np.array([-tan_value, height])
    dright_vector =np.array([tan_value, height])
    
    return (uleft_vector, uright_vector, dleft_vector, dright_vector)
    

def get_mid_back_mask(position, cur_pivot, cur_size):
    _, _, _, _, down_point, up_point = get_standard(position, cur_pivot, cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_pivot, cur_size)

    poly = np.array([
        down_point,
        down_point + uright_vector,
        up_point + dright_vector,
        up_point,
        up_point + dleft_vector,
        down_point + uleft_vector
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))
    
    
    return (poly, False, False)

def get_mid_front_mask(position, cur_pivot, cur_size):
    down_pivot_double_point, up_pivot_double_point, down_pivot_point, up_pivot_point, down_point, up_point = get_standard(position, cur_pivot, cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_pivot, cur_size)

    poly = np.array([
        down_point,
        down_point + uright_vector,
        up_point + dright_vector,
        up_pivot_double_point,
        up_point + dleft_vector,
        down_point + uleft_vector
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))
    
    
    return (poly, False, False)

def get_left_front_mask(position, cur_pivot, cur_size):
    down_pivot_double_point, up_pivot_double_point, down_pivot_point, up_pivot_point, down_point, up_point = get_standard(position, cur_pivot, cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_pivot, cur_size)
    
    poly = np.array([
        down_point,
        up_pivot_double_point,
        up_point + dleft_vector,
        down_point + uleft_vector
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))
    
    return (poly, True, True)

def get_right_front_mask(position, cur_pivot, cur_size):
    down_pivot_double_point, up_pivot_double_point, down_pivot_point, up_pivot_point, down_point, up_point = get_standard(position, cur_pivot, cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_pivot, cur_size)

    poly = np.array([
        down_point,
        up_pivot_double_point,
        up_point + dright_vector,
        down_point + uright_vector
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))
    
    return (poly, False, True)

def get_left_back_mask(position, cur_pivot, cur_size):
    down_pivot_double_point, up_pivot_double_point, down_pivot_point, up_pivot_point, down_point, up_point = get_standard(position, cur_pivot, cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_pivot, cur_size)
    
    poly = np.array([
        down_point,
        up_pivot_double_point,
        up_point + dleft_vector,
        down_point + uleft_vector
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))
    
    return (poly, True, True)

def image_slice(image, pts, cur_size, is_side, is_left):
    mask = np.zeros((*image.shape[:2], 4), dtype=np.uint8)
    cv2.fillPoly(mask, [pts], (255, 255, 255, 255))
    masked_image = cv2.bitwise_and(image, mask)
    gray_masked_image = cv2.cvtColor(masked_image, cv2.COLOR_BGRA2GRAY)
    x, y, w, h = cv2.boundingRect(gray_masked_image)
    cropped_image = masked_image[y:y+h, x:x+w]

    resized_image = cropped_image
    return resized_image

#    if(w > cur_size[0]):
#        resized_image = cv2.resize(resized_image, (cur_size[0], resized_image.shape[0]))
#    if(h > cur_size[1]):
#        resized_image = cv2.resize(resized_image, (resized_image.shape[1], cur_size[1]))
#
#
#    if(is_side and int(cur_size[0]/2) < resized_image.shape[1]):
#        resized_image = cv2.resize(cropped_image, (int(cur_size[0]/2), resized_image.shape[0]))
#
#
#    canvas = np.zeros((cur_size[1], cur_size[0], 4), dtype=np.uint8)
#
#    if(is_side == False or is_left):
#        canvas[
#            cur_size[1]-resized_image.shape[0]:cur_size[1],
#            0:resized_image.shape[1]
#            ] = resized_image
#    else:
#        canvas[
#            cur_size[1]-resized_image.shape[0]:cur_size[1],
#            int(cur_size[0]/2):int(cur_size[0]/2)+resized_image.shape[1]
#            ] = resized_image
        
    return canvas


image = cv2.imread('1.png', cv2.IMREAD_UNCHANGED)



def cut_and_save(vector, mask_func, name):
    cur_point = np.array(start_point) + vector

    pts, is_left, is_side = mask_func(cur_point, pivot, size)

    img = image_slice(image, pts, size, is_side, is_left)

    cv2.imwrite(name, img)
    

for cur_y in range(0, y_iter):
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(pivot, size)
    up_step = size[1]  - (size[1] - pivot[1]) * 2
    up_vector = np.array([0, float(cur_y) * -up_step])
    cut_and_save(up_vector, get_mid_front_mask, f'front_mid_x{cur_y + 1}_y{cur_y+1}.png')
    
    for cur_x in range(0, x_iter_left):
        cut_and_save(uleft_vector * float(cur_x) + up_vector, get_left_front_mask, f'front_left_x{cur_x+1}_y{cur_y+1}.png')
    
    for cur_x in range(0, x_iter_right):
        cut_and_save(uright_vector * float(cur_x) + up_vector, get_right_front_mask, f'front_right_{cur_x+1}_y{cur_y+1}.png')


# 결과 이미지 보기
#cv2.imshow('Resized Image', canvas)
#cv2.waitKey(0)
#cv2.destroyAllWindows()
