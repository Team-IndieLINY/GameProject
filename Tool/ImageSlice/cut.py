import cv2
import numpy as np

resoultion = (3587, 2111)
start_point = (1540, 2116)
pivot = (256, 889)
#angle = 124.3907145
angle = 126.86989764584402129685561255909
halfAngle = angle/2
size = (512, 800)
x_iter_left = 6
x_iter_right = 8
y_iter = 2
is_cutting_front = True

de_to_local_pixel_matrix = np.array([
    [1, 0, 0],
    [0, -1, size[1]/2]
])

def create_world_matrix(position, cur_size):
    return np.array([
        [1, 0, position[0] - cur_size[0]/2],
        [0, 1, position[1] - cur_size[1]],
    ])


def get_standard(cur_size):
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)
    
    up_point = np.array([cur_size[0] / 2, cur_size[1] / 2])
    down_point = np.array([cur_size[0] / 2, -cur_size[1] / 2])
    
    up_pivot_point = up_point + dright_vector + dleft_vector
    down_pivot_point = down_point + uright_vector + uleft_vector
    
    return (down_pivot_point, up_pivot_point, down_point, up_point)

def get_each_vector(cur_size):
    tan_value = np.tan(np.deg2rad(180.0 - angle) / 2)
    height = tan_value * (cur_size[0] / 2)
    width = cur_size[0] / 2
    
    uleft_vector = np.array([-width, height])
    uright_vector =np.array([width, height])
    
    dleft_vector = np.array([-width, -height])
    dright_vector =np.array([width, -height])
    
    return (uleft_vector, uright_vector, dleft_vector, dright_vector)

def transform_mask_poly(poly, position, cur_size):
    m = create_world_matrix(position, cur_size)

    for i in range(0, 4):
        p = poly[i][0]
        p = np.dot(de_to_local_pixel_matrix, np.array([p[0], p[1], 1]))
        p = np.dot(m, np.array([p[0], p[1], 1]))

        poly[i][0] = p

    return poly

def get_right_front_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point,
        down_point + uright_vector,
        up_pivot_point + uright_vector,
        up_pivot_point
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), False, True)

def get_left_front_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point,
        down_point + uleft_vector,
        up_pivot_point + uleft_vector,
        up_pivot_point
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), False, True)

def image_slice(image, pts, cur_size, is_side, is_left):
    mask = np.zeros((*image.shape[:2], 4), dtype=np.uint8)
    cv2.fillPoly(mask, [pts], (255, 255, 255, 255))
    masked_image = cv2.bitwise_and(image, mask)
    gray_masked_image = cv2.cvtColor(masked_image, cv2.COLOR_BGRA2GRAY)
    x, y, w, h = cv2.boundingRect(gray_masked_image)
    cropped_image = masked_image[y:y+h, x:x+w]

    resized_image = cropped_image
    return resized_image

    #if(w > cur_size[0]):
    #    resized_image = cv2.resize(resized_image, (cur_size[0], resized_image.shape[0]))
    #if(h > cur_size[1]):
    #    resized_image = cv2.resize(resized_image, (resized_image.shape[1], cur_size[1]))
#
#
    #if(is_side and int(cur_size[0]/2) < resized_image.shape[1]):
    #    resized_image = cv2.resize(cropped_image, (int(cur_size[0]/2), resized_image.shape[0]))
#
#
    #canvas = np.zeros((cur_size[1], cur_size[0], 4), dtype=np.uint8)
#
    #if(is_side == False or is_left):
    #    canvas[
    #        cur_size[1]-resized_image.shape[0]:cur_size[1],
    #        0:resized_image.shape[1]
    #        ] = resized_image
    #else:
    #    canvas[
    #        cur_size[1]-resized_image.shape[0]:cur_size[1],
    #        0:resized_image.shape[1]
    #        ] = resized_image
        
    return canvas


image = cv2.imread('1.png', cv2.IMREAD_UNCHANGED)

def cut_and_save(cur_point, mask_func, name):
    pts, is_left, is_side = mask_func(cur_point, size)

    img = image_slice(image, pts, size, is_side, is_left)

    try:
        cv2.imwrite(name, img)
    except:
        print("error")


#cut_and_save(np.array([0, 0]), get_right_front_mask, "test.png")


down_pivot_point, up_pivot_point, down_point, up_point = get_standard(size)
to_up_vector = (up_pivot_point - down_point)
to_up_vector[1] = to_up_vector[1] * -1

for cur_y in range(0, y_iter):
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(size)
    print(to_up_vector * cur_y)
    cur_pos_left = np.array([start_point[0], start_point[1]]) + to_up_vector * cur_y
    cur_pos_right = np.array([start_point[0], start_point[1]]) + to_up_vector * cur_y
    
    for cur_x in range(0, x_iter_left):
        cut_and_save(cur_pos_left, get_left_front_mask, f'front_left_x{cur_x+1}_y{cur_y+1}.png')
        cur_pos_left = cur_pos_left + np.array([uleft_vector[0], -uleft_vector[1]])
    
    for cur_x in range(0, x_iter_right):
        cut_and_save(cur_pos_right, get_right_front_mask, f'front_right_x{cur_x+1}_y{cur_y+1}.png')
        cur_pos_right = cur_pos_right + np.array([uright_vector[0], -uright_vector[1]])



# 결과 이미지 보기
#cv2.imshow('Resized Image', canvas)
#cv2.waitKey(0)
#cv2.destroyAllWindows()
